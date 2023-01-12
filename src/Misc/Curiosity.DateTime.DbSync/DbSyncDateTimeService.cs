using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.DAL;
using Curiosity.Tools;

namespace Curiosity.DateTime.DbSync
{
    /// <summary>
    /// Date time service that sync time with database.
    /// </summary>
    public class DbSyncDateTimeService : IDateTimeService
    {
        private TimeCorrectionParams _timeCorrectionParams;

        private readonly ICuriosityDataContextFactory _dataContextFactory;

        /// <inheritdoc cref="DbSyncDateTimeService"/>
        public DbSyncDateTimeService(ICuriosityDataContextFactory dataContextFactor)
        {
            if (dataContextFactor == null) throw new ArgumentNullException(nameof(dataContextFactor));
            
            _dataContextFactory = dataContextFactor ?? throw new ArgumentNullException(nameof(dataContextFactor));
            _timeCorrectionParams = TimeCorrectionParams.ForEqual();
        }

        /// <summary>
        /// Inits service on demand.
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            // don't consider request time to DB
            // we don't need high accuracy
            using (var context = _dataContextFactory.CreateContext())
            {
                var serverTime = await context.GetImmediateServerTimeUtcAsync(cancellationToken);
                var localTime = System.DateTime.UtcNow;

                if (serverTime > localTime)
                {
                    _timeCorrectionParams = TimeCorrectionParams.ForBigger(serverTime - localTime);
                }
                else if (serverTime < localTime)
                {
                    _timeCorrectionParams = TimeCorrectionParams.ForSmaller(localTime - serverTime);
                }
                else
                {
                    _timeCorrectionParams = TimeCorrectionParams.ForEqual();
                }
            }
        }

        /// <inheritdoc />
        public System.DateTime GetCurrentTimeUtc()
        {
            var localUtc = System.DateTime.UtcNow;

            switch (_timeCorrectionParams.CorrectionType)
            {
                case TimeCorrectionType.Negative:
                    return System.DateTime.SpecifyKind(
                        localUtc - _timeCorrectionParams.Correction, 
                        DateTimeKind.Utc);
                
                case TimeCorrectionType.Positive:
                    return System.DateTime.SpecifyKind(
                        localUtc + _timeCorrectionParams.Correction, 
                        DateTimeKind.Utc);      
                
                case TimeCorrectionType.None:
                    return System.DateTime.SpecifyKind(localUtc, DateTimeKind.Utc);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(_timeCorrectionParams.CorrectionType));
            }
        }
        
        /// <summary>
        /// Params for reducing difference between server and app time.
        /// </summary>
        private struct TimeCorrectionParams
        {
            private TimeCorrectionParams(TimeSpan correction, TimeCorrectionType correctionType)
            {
                Correction = correction;
                CorrectionType = correctionType;
            }

            /// <summary>
            /// Difference between server time (time in database) and local (app time) in time value
            /// </summary>
            public TimeSpan Correction { get; }
            
            /// <summary>
            /// Correction type  
            /// </summary>
            public TimeCorrectionType CorrectionType { get; } 

            public static TimeCorrectionParams ForSmaller(TimeSpan shift)
            {
                return new TimeCorrectionParams(shift, TimeCorrectionType.Negative);
            }
            
            public static TimeCorrectionParams ForBigger(TimeSpan shift)
            {
                return new TimeCorrectionParams(shift, TimeCorrectionType.Positive);
            }

            public static TimeCorrectionParams ForEqual()
            {
                return new TimeCorrectionParams(TimeSpan.Zero, TimeCorrectionType.None);
            }
        }
        
        /// <summary>
        /// Difference between server time (time in database) and local (app time).
        /// </summary>
        private enum TimeCorrectionType
        {
            /// <summary>
            /// Server and app times are equal
            /// </summary>
            None,
            
            /// <summary>
            /// Server time is smaller than local
            /// </summary>
            Negative,
            
            /// <summary>
            /// Server time is bigger than local
            /// </summary>
            Positive
        }
    }
}
