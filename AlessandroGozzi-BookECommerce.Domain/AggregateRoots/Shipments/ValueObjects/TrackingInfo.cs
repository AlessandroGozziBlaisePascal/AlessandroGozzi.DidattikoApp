using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects
{
    public sealed class TrackingInfo
    {
        public string Carrier { get; init; }
        public string TrackingCode { get; private set; }
        public string? TrackingUrl { get; private set; }

        private TrackingInfo(string carrier, string trackingCode, string? trackingUrl)
        {
            Carrier = carrier;
            TrackingCode = trackingCode;
            TrackingUrl = trackingUrl;
        }

        public static Result<TrackingInfo> Create(string carrier, string trackingCode, string? trackingUrl = null)
        {
            if (string.IsNullOrWhiteSpace(carrier))
                return Result.Failure<TrackingInfo>(new Error("Carrier", "Carrier cannot be empty.", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(trackingCode) || trackingCode.Length < 5)
                return Result.Failure<TrackingInfo>(new Error("Tracking code","Tracking code cannot be empty.", ErrorType.Validation));
            var trackingInfo = new TrackingInfo(carrier, trackingCode, trackingUrl);
            return Result.Success(trackingInfo);
        }

        public static TrackingInfo SetUntracked() => new TrackingInfo("PosteItaliane_PieghiLibriOrdinario", "Untracked", null);
    }
}
