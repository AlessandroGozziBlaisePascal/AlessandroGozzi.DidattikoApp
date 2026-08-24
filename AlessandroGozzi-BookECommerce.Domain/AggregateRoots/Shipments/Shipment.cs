using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.Events;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments
{
    public class Shipment: AggregateRoot
    {
        public Guid VendorId { get; private set; }
        public Guid BuyerId { get; private set; }
        public Address ShippingAddress { get; private set; }
        public Guid OrderId { get; private set; }
        public ShipmentStatus Status { get; private set; }
        public ShippingType ShippingType { get; private set; }
        public Money SubTotal { get; private set; }
        public TrackingInfo? TrackingInfo { get; private set; }
        public DateTime? ShippedAtUtc { get; private set; }

        private Shipment() { } // Per EF Core
        private Shipment(Guid vendorId, Guid orderId, ShippingType type, Money subTotal, Guid buyerId, Address address)
        {
            VendorId = vendorId;
            OrderId = orderId;
            Status = ShipmentStatus.Preparing;
            ShippingType = type;
            SubTotal = subTotal;
            ShippingAddress = address;
            BuyerId = buyerId;
        }

        public static Result<Shipment> Create(Guid vendorId, Guid orderId, ShippingType type, Money subTotal, Guid buyerId, Address address)
        {
            if (vendorId == Guid.Empty)
                return Result.Failure<Shipment>(new Error("Shipment.VendorId", "VendorId cannot be empty.", ErrorType.Validation));
            if (orderId == Guid.Empty)
                return Result.Failure<Shipment>(new Error("Shipment.OrderId", "OrderId cannot be empty.", ErrorType.Validation));
            if(subTotal == null || subTotal.Amount <= 0)
                return Result.Failure<Shipment>(new Error("Shipment.SubTotal", "SubTotal must be greater than zero.", ErrorType.Validation));
            if (buyerId == Guid.Empty)
                return Result.Failure<Shipment>(new Error("Shipment.BuyerId", "BuyerId cannot be empty.", ErrorType.Validation));
            if(address == null)
                return Result.Failure<Shipment>(new Error("Shipment.ShippingAddress","Address of shipping cannot be null",ErrorType.Validation));

            var shipment = new Shipment(vendorId, orderId, type, subTotal, buyerId, address);

            shipment.Raise(new ShipmentCreatedEvent(
                Id: shipment.Id,
                OrderId: shipment.OrderId,
                SellerId: shipment.VendorId
            ));
            return Result.Success(shipment);
        }

        public Result MarkAsShipped(Guid requestingVendorId, string? trackingNumber = null, string? carrier = null)
        {
            if (VendorId != requestingVendorId)
                return Result.Failure(new Error("Shipment.Unauthorized", "Non sei autorizzato a modificare questa spedizione.", ErrorType.PermissionDenied));

            if (Status == ShipmentStatus.Cancelled || Status == ShipmentStatus.Delivered)
                return Result.Failure(new Error("Shipment.InvalidState", $"Impossibile aggiornare il tracking per una spedizione nello stato {Status}.", ErrorType.StatusConflict));

            bool hasCarrier = !string.IsNullOrWhiteSpace(carrier);
            bool hasTrackingNumber = !string.IsNullOrWhiteSpace(trackingNumber);
            if (hasCarrier && hasTrackingNumber)
            {
                var trackingInfoResult = TrackingInfo.Create(carrier!, trackingNumber!);
                if (trackingInfoResult.IsFailure)
                    return Result.Failure(trackingInfoResult.Error);
                TrackingInfo = trackingInfoResult.Value;
            }
            else if (hasCarrier ^ hasTrackingNumber)
                return Result.Failure(new Error("Tracking info", "Cannot have only one parameter: 0=untracked, 2=tracked", ErrorType.Validation));
            else
            {
                TrackingInfo = TrackingInfo.SetUntracked();
            }

            if (Status == ShipmentStatus.Preparing)
            {
                Status = ShipmentStatus.Shipped;
                ShippedAtUtc = DateTime.UtcNow;
            }

            Raise(new ShipmentShippedEvent(
                Id: Id,
                OrderId: OrderId,
                SellerId: VendorId,
                TrackInfo: TrackingInfo
            ));

            return Result.Success();
        }

        public Result MarkAsDelivered(Guid requestingBuyerId)
        {
            if (Status != ShipmentStatus.Shipped)
                return Result.Failure(new Error("Shipment.InvalidState", $"Impossibile contrassegnare come consegnata una spedizione nello stato {Status}.", ErrorType.StatusConflict));
            if (BuyerId != requestingBuyerId)
                return Result.Failure(new Error("Shipment.PermissionDenied", "You are not the buyer", ErrorType.PermissionDenied));
            Status = ShipmentStatus.Delivered;
            Raise(new ShipmentDeliveredEvent(
                Id: Id,
                OrderId: OrderId,
                SellerId: VendorId
            ));
            return Result.Success();
        }

        public Result CancelShipment(Guid requestingVendorId)
        {
            if (VendorId != requestingVendorId)
                return Result.Failure(new Error("Shipment.Unauthorized", "Non sei autorizzato a cancellare questa spedizione.", ErrorType.PermissionDenied));
            if (Status == ShipmentStatus.Delivered || Status == ShipmentStatus.Cancelled || Status == ShipmentStatus.Shipped)
                return Result.Failure(new Error("Shipment.InvalidState", "Impossibile cancellare una spedizione già consegnata o già cancellata.", ErrorType.StatusConflict));
            Status = ShipmentStatus.Cancelled;
            Raise(new ShipmentCancelledEvent(
                Id: Id,
                OrderId: OrderId,
                SellerId: VendorId
            ));
            return Result.Success();
        }

    }
}
