using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments.Events;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.AggregateRoots.Shipments
{
    public class ShipmentTests
    {
        private readonly Guid _vendorId = Guid.NewGuid();
        private readonly Guid _orderId = Guid.NewGuid();
        private readonly Guid _buyerId = Guid.NewGuid();
        private readonly ShippingType _shippingType = ShippingType.Express;
        private readonly Money _subTotal = Money.Create(25.00m).Value;
        private readonly Address _address = Address.Create("Via Roma", "10", "Milano", "20100").Value;

        private Shipment CreateValidShipment()
        {
            return Shipment.Create(_vendorId, _orderId, _shippingType, _subTotal, _buyerId, _address).Value;
        }

        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndRaiseShipmentCreatedEvent()
        {
            var result = Shipment.Create(_vendorId, _orderId, _shippingType, _subTotal, _buyerId, _address);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.VendorId.Should().Be(_vendorId);
            result.Value.OrderId.Should().Be(_orderId);
            result.Value.BuyerId.Should().Be(_buyerId);
            result.Value.ShippingType.Should().Be(_shippingType);
            result.Value.SubTotal.Should().Be(_subTotal);
            result.Value.ShippingAddress.Should().Be(_address);
            result.Value.Status.Should().Be(ShipmentStatus.Preparing);
            result.Value.TrackingInfo.Should().BeNull();
            result.Value.ShippedAtUtc.Should().BeNull();

            result.Value._domainEvents.Should().ContainSingle(e => e is ShipmentCreatedEvent);
        }

        [Fact]
        public void Create_WhenVendorIdIsEmpty_ShouldReturnFailure()
        {
            var result = Shipment.Create(Guid.Empty, _orderId, _shippingType, _subTotal, _buyerId, _address);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.VendorId");
            result.Error.Description.Should().Be("VendorId cannot be empty.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenOrderIdIsEmpty_ShouldReturnFailure()
        {
            var result = Shipment.Create(_vendorId, Guid.Empty, _shippingType, _subTotal, _buyerId, _address);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.OrderId");
            result.Error.Description.Should().Be("OrderId cannot be empty.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenSubTotalIsNull_ShouldReturnFailure()
        {
            var result = Shipment.Create(_vendorId, _orderId, _shippingType, null!, _buyerId, _address);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.SubTotal");
            result.Error.Description.Should().Be("SubTotal must be greater than zero.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenSubTotalIsZeroOrNegative_ShouldReturnFailure()
        {
            var zeroSubTotal = Money.Create(0m).Value;

            var result = Shipment.Create(_vendorId, _orderId, _shippingType, zeroSubTotal, _buyerId, _address);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.SubTotal");
            result.Error.Description.Should().Be("SubTotal must be greater than zero.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenBuyerIdIsEmpty_ShouldReturnFailure()
        {
            var result = Shipment.Create(_vendorId, _orderId, _shippingType, _subTotal, Guid.Empty, _address);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.BuyerId");
            result.Error.Description.Should().Be("BuyerId cannot be empty.");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenAddressIsNull_ShouldReturnFailure()
        {
            var result = Shipment.Create(_vendorId, _orderId, _shippingType, _subTotal, _buyerId, null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.ShippingAddress");
            result.Error.Description.Should().Be("Address of shipping cannot be null");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // MARK AS SHIPPED TESTS
        // ==========================================
        [Fact]
        public void MarkAsShipped_WhenRequestingVendorIsNotOwner_ShouldReturnUnauthorizedFailure()
        {
            var shipment = CreateValidShipment();
            var unauthorizedVendorId = Guid.NewGuid();

            var result = shipment.MarkAsShipped(unauthorizedVendorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.Unauthorized");
            result.Error.Description.Should().Be("Non sei autorizzato a modificare questa spedizione.");
            result.Error.Type.Should().Be(ErrorType.PermissionDenied);
        }

        [Fact]
        public void MarkAsShipped_WhenStatusIsCancelledOrDelivered_ShouldReturnInvalidStateFailure()
        {
            var shipment = CreateValidShipment();
            shipment.CancelShipment(_vendorId);

            var result = shipment.MarkAsShipped(_vendorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.InvalidState");
            result.Error.Description.Should().StartWith("Impossibile aggiornare il tracking per una spedizione nello stato");
        }

        [Fact]
        public void MarkAsShipped_WithBothCarrierAndTrackingNumber_ShouldSetTrackingAndRaiseEvent()
        {
            var shipment = CreateValidShipment();
            shipment._domainEvents.Clear();
            const string carrier = "DHL";
            const string trackingNumber = "1234567";

            var result = shipment.MarkAsShipped(_vendorId, trackingNumber, carrier);

            result.IsSuccess.Should().BeTrue();
            shipment.Status.Should().Be(ShipmentStatus.Shipped);
            shipment.ShippedAtUtc.Should().NotBeNull();
            shipment.TrackingInfo.Should().NotBeNull();
            shipment.TrackingInfo!.Carrier.Should().Be(carrier);
            shipment.TrackingInfo.TrackingCode.Should().Be(trackingNumber);
            shipment._domainEvents.Should().ContainSingle(e => e is ShipmentShippedEvent);
        }

        [Fact]
        public void MarkAsShipped_WithOnlyOneTrackingParameter_ShouldReturnValidationFailure()
        {
            var shipment = CreateValidShipment();

            var result = shipment.MarkAsShipped(_vendorId, "1234567", null);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tracking info");
            result.Error.Description.Should().Be("Cannot have only one parameter: 0=untracked, 2=tracked");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void MarkAsShipped_WithoutTrackingParameters_ShouldSetUntrackedAndRaiseEvent()
        {
            var shipment = CreateValidShipment();
            shipment._domainEvents.Clear();

            var result = shipment.MarkAsShipped(_vendorId);

            result.IsSuccess.Should().BeTrue();
            shipment.Status.Should().Be(ShipmentStatus.Shipped);
            shipment.ShippedAtUtc.Should().NotBeNull();
            shipment.TrackingInfo.Should().NotBeNull();
            shipment.TrackingInfo!.TrackingCode.Should().Be("Untracked");
            shipment._domainEvents.Should().ContainSingle(e => e is ShipmentShippedEvent);
        }

        // ==========================================
        // MARK AS DELIVERED TESTS
        // ==========================================
        [Fact]
        public void MarkAsDelivered_WhenStatusIsNotShipped_ShouldReturnInvalidStateFailure()
        {
            var shipment = CreateValidShipment(); // Stato: Preparing

            var result = shipment.MarkAsDelivered(_buyerId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.InvalidState");
            result.Error.Description.Should().StartWith("Impossibile contrassegnare come consegnata");
        }

        [Fact]
        public void MarkAsDelivered_WhenRequestingBuyerIsNotOwner_ShouldReturnPermissionDeniedFailure()
        {
            var shipment = CreateValidShipment();
            shipment.MarkAsShipped(_vendorId);
            var unauthorizedBuyerId = Guid.NewGuid();

            var result = shipment.MarkAsDelivered(unauthorizedBuyerId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.PermissionDenied");
            result.Error.Description.Should().Be("You are not the buyer");
            result.Error.Type.Should().Be(ErrorType.PermissionDenied);
        }

        [Fact]
        public void MarkAsDelivered_WhenValidBuyerAndShippedStatus_ShouldSetDeliveredAndRaiseEvent()
        {
            var shipment = CreateValidShipment();
            shipment.MarkAsShipped(_vendorId);
            shipment._domainEvents.Clear();

            var result = shipment.MarkAsDelivered(_buyerId);

            result.IsSuccess.Should().BeTrue();
            shipment.Status.Should().Be(ShipmentStatus.Delivered);
            shipment._domainEvents.Should().ContainSingle(e => e is ShipmentDeliveredEvent);
        }

        // ==========================================
        // CANCEL SHIPMENT TESTS
        // ==========================================
        [Fact]
        public void CancelShipment_WhenRequestingVendorIsNotOwner_ShouldReturnUnauthorizedFailure()
        {
            var shipment = CreateValidShipment();
            var unauthorizedVendorId = Guid.NewGuid();

            var result = shipment.CancelShipment(unauthorizedVendorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.Unauthorized");
            result.Error.Description.Should().Be("Non sei autorizzato a cancellare questa spedizione.");
            result.Error.Type.Should().Be(ErrorType.PermissionDenied);
        }

        [Theory]
        [InlineData(ShipmentStatus.Shipped)]
        [InlineData(ShipmentStatus.Delivered)]
        public void CancelShipment_WhenStatusIsShippedOrDelivered_ShouldReturnInvalidStateFailure(ShipmentStatus targetStatus)
        {
            var shipment = CreateValidShipment();
            shipment.MarkAsShipped(_vendorId);
            if (targetStatus == ShipmentStatus.Delivered)
            {
                shipment.MarkAsDelivered(_buyerId);
            }

            var result = shipment.CancelShipment(_vendorId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Shipment.InvalidState");
            result.Error.Description.Should().Be("Impossibile cancellare una spedizione già consegnata o già cancellata.");
        }

        [Fact]
        public void CancelShipment_WhenPreparingAndValidVendor_ShouldSetCancelledAndRaiseEvent()
        {
            var shipment = CreateValidShipment();
            shipment._domainEvents.Clear();

            var result = shipment.CancelShipment(_vendorId);

            result.IsSuccess.Should().BeTrue();
            shipment.Status.Should().Be(ShipmentStatus.Cancelled);
            shipment._domainEvents.Should().ContainSingle(e => e is ShipmentCancelledEvent);
        }
    }
}
