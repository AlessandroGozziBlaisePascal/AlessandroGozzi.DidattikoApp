using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record ShipmentDto(
        Guid Id,
        Guid VendorId,
        Guid BuyerId,
        AddressDto ShipmentAddress,
        Guid OrderId,
        ShipmentStatus Status,
        ShippingType Type,
        decimal Amount,
        TrackingInfoDto? TrackingInfo,
        DateTime? ShippedAt
        );
}
