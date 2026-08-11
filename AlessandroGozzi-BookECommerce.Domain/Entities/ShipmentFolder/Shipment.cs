using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder
{
    public class Shipment: Entity
    {
        public Guid VendorId { get; private set; }
        public Guid OrderId { get; private set; }
        public ShipmentStatus Status { get; private set; }
        public ShippingType ShippingType { get; private set; }
        public TrackingInfo? TrackingInfo { get; private set; }
        public DateTime? ShippedAtUtc { get; private set; }

        private Shipment() { } // Per EF Core
        private Shipment(Guid vendorId, Guid orderId, ShippingType type)
        {
            VendorId = vendorId;
            OrderId = orderId;
            Status = ShipmentStatus.Preparing;
            ShippingType = type;
        }

        public static Result<Shipment> Create(Guid vendorId, Guid orderId, ShippingType type)
        {
            if (vendorId == Guid.Empty)
                return Result.Failure<Shipment>(new Error("Shipment.VendorId", "VendorId cannot be empty.", ErrorType.Validation));
            if (orderId == Guid.Empty)
                return Result.Failure<Shipment>(new Error("Shipment.OrderId", "OrderId cannot be empty.", ErrorType.Validation));
            var shipment = new Shipment(vendorId, orderId, type);

            shipment.Raise(new ShipmentCreatedDomainEvent(
                Id: shipment.Id,
                OrderId: shipment.OrderId,
                SellerId: shipment.VendorId
            ));
            return Result.Success(shipment);
        }

        public Result UpdateTracking(TrackingInfo newTracking, Guid requestingVendorId)
        {
            if (VendorId != requestingVendorId)
                return Result.Failure(new Error("Shipment.Unauthorized", "Non sei autorizzato a modificare questa spedizione.", ErrorType.PermissionDenied));

            if (Status == ShipmentStatus.Cancelled || Status == ShipmentStatus.Delivered)
                return Result.Failure(new Error("Shipment.InvalidState", $"Impossibile aggiornare il tracking per una spedizione nello stato {Status}.", ErrorType.StatusConflict));

            TrackingInfo = newTracking;

            if (Status == ShipmentStatus.Preparing)
            {
                Status = ShipmentStatus.Shipped;
                ShippedAtUtc = DateTime.UtcNow;
            }

            Raise(new ShipmentTrackingUpdatedDomainEvent(
                Id: Id,
                OrderId: OrderId,
                SellerId: VendorId,
                TrackInfo: TrackingInfo
            ));

            return Result.Success();
        }

        public Result MarkAsDelivered()
        {
            if (Status != ShipmentStatus.Shipped)
                return Result.Failure(new Error("Shipment.InvalidState", $"Impossibile contrassegnare come consegnata una spedizione nello stato {Status}.", ErrorType.StatusConflict));
            Status = ShipmentStatus.Delivered;
            Raise(new ShipmentDeliveredDomainEvent(
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
            if (Status == ShipmentStatus.Delivered || Status == ShipmentStatus.Cancelled)
                return Result.Failure(new Error("Shipment.InvalidState", "Impossibile cancellare una spedizione già consegnata o già cancellata.", ErrorType.StatusConflict));
            Status = ShipmentStatus.Cancelled;
            Raise(new ShipmentCancelledDomainEvent(
                Id: Id,
                OrderId: OrderId,
                SellerId: VendorId
            ));
            return Result.Success();
        }

    }
}
