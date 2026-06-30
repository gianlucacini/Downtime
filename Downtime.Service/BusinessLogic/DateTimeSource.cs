using Serilog;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace Downtime.Service.BusinessLogic
{
    /// <summary>
    /// This class provides a temper resistant datetime source for the Downtime Service.
    /// 
    /// Get the current date time from an ntp server, then updates it using the running stopwatch (when internet is blocked)
    /// CONTEXT:
    /// using the windows local time is not possible, as the user could change date and time, thus bypassing the block.
    /// </summary>
    public class DateTimeSource
    {
        public DateTimeSource(ILogger logger, SntpClient sntpClient)
        {
            _logger = logger;
            _sntpClient = sntpClient;
        }

        private readonly ILogger _logger;
        private readonly SntpClient _sntpClient;
        Stopwatch StopWatch { get; set; } = new Stopwatch();

        Nullable<DateTime> now;
        public Nullable<DateTime> Now(String timeZoneID)
        {
            if (now is null)
            {
                now = GetUtcDateTime();

                if (now is null)
                {
                    //Could not retrieve date and time remotely
                    StopWatch.Stop();

                    now = null;
                }
                else
                {
                    //convert datetime from utc to local
                    DateTime localDt = TimeZoneInfo.ConvertTimeFromUtc(now.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneID));

                    StopWatch.Restart();

                    now = new DateTime(localDt.Year, localDt.Month, localDt.Day, localDt.Hour, localDt.Minute, localDt.Second);

                    _logger.Information($"Local Datetime of Timezone {timeZoneID} is {now}");

                }
            }
            else
            {
                //calculate time elapsed since last stopwatch start
                long elaps = StopWatch.ElapsedMilliseconds;

                //and add elapsed milliseconds to the last datetime retrieved from the server
                now = now.Value.AddMilliseconds(elaps);

                _logger.Information($"Calculated Datetime of Timezone {timeZoneID} is {now}");

                StopWatch.Restart();
            }

            return now;
        }

        Nullable<DateTime> GetUtcDateTime()
        {
            _logger.Information("Retrieving UTC DateTime from NTP...");

            int tryNum = 0;

            do
            {
                try
                {
                    tryNum++;

                    DateTime? date = _sntpClient.GetUtcDateTime();

                    if (date.HasValue)
                    {
                        _logger.Information($"UTC DateTime Found. Result = {date.Value}");
                        return date.Value;
                    }
                }
                catch (SocketException se)
                {
                    _logger.Error(se, $"Failed to retreive current datetime from ntp server. Try Number {tryNum}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Exception while retreiving datetime from ntp server. Try Number {tryNum}");
                }
                finally
                {
                    System.Threading.Thread.Sleep(4000);
                }

            } while (tryNum < 5);

            return null;
        }
    }
}
