using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.ShipmentFolder
{
    public class ShipmentTests
    {
        #region TrackingInfo Tests

        [Theory]
        [InlineData("DHL", "TRACK12345", "https://dhl.com/track/12345")]
        [InlineData("Poste Italiane", "12345", null)]
        public void TrackingInfo_Create_WithValidParameters_ShouldReturnSuccessResult(
            string carrier,
            string trackingCode,
            string? trackingUrl)
        {
            var result = TrackingInfo.Create(carrier, trackingCode, trackingUrl);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Carrier.Should().Be(carrier);
            result.Value.TrackingCode.Should().Be(trackingCode);
            result.Value.TrackingUrl.Should().Be(trackingUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void TrackingInfo_Create_WithInvalidCarrier_ShouldReturnFailureError(string? invalidCarrier)
        {
            var result = TrackingInfo.Create(invalidCarrier!, "TRACK12345", null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Carrier");
            result.Error.Description.Should().Be("Carrier cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1234")]
        public void TrackingInfo_Create_WithInvalidTrackingCode_ShouldReturnFailureError(string? invalidTrackingCode)
        {
            var result = TrackingInfo.Create("DHL", invalidTrackingCode!, null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tracking code");
            result.Error.Description.Should().Be("Tracking code cannot be empty.");
        }

        #endregion

        #region Shipment Tests

        [Fact]
        public void UpdateTracking_WhenVendorIdDoesNotMatch_ShouldReturnUnauthorizedFailure()
        {
            var ownerVendorId = Guid.NewGuid();
            var unauthorizedVendorId = Guid.NewGuid();
            var shipment = CreateTestShipment(ownerVendorId, ShipmentStatus.Preparing);

            var result = shipment.UpdateTracking(CreateValidTrackingInfo(), unauthorizedVendorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.Unauthorized");
            result.Error.Description.Should().Be("Non sei autorizzato a modificare questa spedizione.");
        }

        [Theory]
        [InlineData(ShipmentStatus.Cancelled)]
        [InlineData(ShipmentStatus.Delivered)]
        public void UpdateTracking_WhenStatusIsInvalid_ShouldReturnInvalidStateFailure(ShipmentStatus invalidStatus)
        {
            var vendorId = Guid.NewGuid();
            var shipment = CreateTestShipment(vendorId, invalidStatus);

            var result = shipment.UpdateTracking(CreateValidTrackingInfo(), vendorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.InvalidState");
            result.Error.Description.Should().Be($"Impossibile aggiornare il tracking per una spedizione nello stato {invalidStatus}.");
        }

        [Fact]
        public void UpdateTracking_WhenStatusIsPreparing_ShouldUpdateStatusToShippedAndSetShippedAtUtc()
        {
            var vendorId = Guid.NewGuid();
            var shipment = CreateTestShipment(vendorId, ShipmentStatus.Preparing);
            var trackingInfo = CreateValidTrackingInfo();
            var beforeUtc = DateTime.UtcNow;

            var result = shipment.UpdateTracking(trackingInfo, vendorId);

            result.IsSuccess.Should().BeTrue();
            shipment.TrackingInfo.Should().Be(trackingInfo);
            shipment.Status.Should().Be(ShipmentStatus.Shipped);
            shipment.ShippedAtUtc.Should().NotBeNull();
            shipment.ShippedAtUtc.Should().BeOnOrAfter(beforeUtc);
        }

        [Fact]
        public void UpdateTracking_WhenSuccessful_ShouldRaiseShipmentTrackingUpdatedDomainEvent()
        {
            var vendorId = Guid.NewGuid();
            var shipment = CreateTestShipment(vendorId, ShipmentStatus.Preparing);
            var trackingInfo = CreateValidTrackingInfo();

            var result = shipment.UpdateTracking(trackingInfo, vendorId);

            result.IsSuccess.Should().BeTrue();
            shipment._domainEvents.Should().ContainSingle(e => e is ShipmentTrackingUpdatedEvent);

            var domainEvent = shipment._domainEvents.OfType<ShipmentTrackingUpdatedEvent>().Single();
            domainEvent.Id.Should().Be(shipment.Id);
            domainEvent.OrderId.Should().Be(shipment.OrderId);
            domainEvent.SellerId.Should().Be(shipment.VendorId);
            domainEvent.TrackInfo.Should().Be(trackingInfo);
        }

        #endregion

        #region Helper Methods

        private static TrackingInfo CreateValidTrackingInfo()
        {
            return TrackingInfo.Create("DHL", "TRACK12345", null).Value;
        }

        private static Shipment CreateTestShipment(Guid vendorId, ShipmentStatus status)
        {
            var shipment = (Shipment)Activator.CreateInstance(typeof(Shipment), true)!;

            typeof(Shipment).GetProperty(nameof(Shipment.Id))?.SetValue(shipment, Guid.NewGuid());
            typeof(Shipment).GetProperty(nameof(Shipment.VendorId))?.SetValue(shipment, vendorId);
            typeof(Shipment).GetProperty(nameof(Shipment.OrderId))?.SetValue(shipment, Guid.NewGuid());
            typeof(Shipment).GetProperty(nameof(Shipment.Status))?.SetValue(shipment, status);

            return shipment;
        }

        #endregion
    }
}
