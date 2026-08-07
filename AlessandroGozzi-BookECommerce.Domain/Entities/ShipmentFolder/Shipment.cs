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
        public TrackingInfo? TrackingInfo { get; private set; }
        public DateTime? ShippedAtUtc { get; private set; }

        private Shipment() { } // Per EF Core

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

    }
}
