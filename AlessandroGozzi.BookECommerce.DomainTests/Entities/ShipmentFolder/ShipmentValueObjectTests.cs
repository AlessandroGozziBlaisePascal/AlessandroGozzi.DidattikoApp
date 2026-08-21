using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.ShipmentFolder
{
    public class ShipmentValueObjectTests
    {
        private const string ValidCarrier = "Poste Italiane";
        private const string ValidTrackingCode = "1234567";
        private const string ValidTrackingUrl = "https://example.com/track/1234567";

        // ==========================================
        // CREATE TESTS (SUCCESS)
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            var result = TrackingInfo.Create(ValidCarrier, ValidTrackingCode, ValidTrackingUrl);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Carrier.Should().Be(ValidCarrier);
            result.Value.TrackingCode.Should().Be(ValidTrackingCode);
            result.Value.TrackingUrl.Should().Be(ValidTrackingUrl);
        }

        [Fact]
        public void Create_WithoutTrackingUrl_ShouldReturnSuccessWithNullUrl()
        {
            var result = TrackingInfo.Create(ValidCarrier, ValidTrackingCode);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Carrier.Should().Be(ValidCarrier);
            result.Value.TrackingCode.Should().Be(ValidTrackingCode);
            result.Value.TrackingUrl.Should().BeNull();
        }

        // ==========================================
        // CREATE TESTS (VALIDATIONS / FAILURES)
        // ==========================================
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WhenCarrierIsNullOrEmptyOrWhitespace_ShouldReturnFailure(string? invalidCarrier)
        {
            var result = TrackingInfo.Create(invalidCarrier!, ValidTrackingCode, ValidTrackingUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Carrier");
            result.Error.Description.Should().Be("Carrier cannot be empty.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("1234")]
        public void Create_WhenTrackingCodeIsInvalidOrShorterThanFiveChars_ShouldReturnFailure(string? invalidTrackingCode)
        {
            var result = TrackingInfo.Create(ValidCarrier, invalidTrackingCode!, ValidTrackingUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tracking code");
            result.Error.Description.Should().Be("Tracking code cannot be empty.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // SET UNTRACKED TESTS
        // ==========================================
        [Fact]
        public void SetUntracked_ShouldReturnDefaultUntrackedTrackingInfo()
        {
            var trackingInfo = TrackingInfo.SetUntracked();

            trackingInfo.Should().NotBeNull();
            trackingInfo.Carrier.Should().Be("PosteItaliane_PieghiLibriOrdinario");
            trackingInfo.TrackingCode.Should().Be("Untracked");
            trackingInfo.TrackingUrl.Should().BeNull();
        }
    }
}
