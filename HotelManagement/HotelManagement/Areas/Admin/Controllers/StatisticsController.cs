using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]

    public class StatisticsController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("RevenueStatistics")]
        public IActionResult RevenueStatistics(DateOnly? fromDate, DateOnly?toDate, string statisticType)
        {
            var query = db.HoaDons.AsQueryable();
            if (fromDate.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu <= toDate.Value);
            }
 
           var result = query.GroupBy(hd => hd.NgayLapPhieu) 
                .Select
                (g => new
                {
                    Date = g.Key, 
                    TotalRevenue = g.Sum(hd => hd.TongTienThu)
                }).ToList();
            return View(result);

        }
        [Route("RevenueStatisticsMonth")]
        public IActionResult RevenueStatisticsMonth(DateOnly? fromMonth, DateOnly? toMonth) 
        {
            var query = db.HoaDons.AsQueryable();

            if (fromMonth.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu >= fromMonth.Value);
            }

            if (toMonth.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu <= toMonth.Value);
            }

            var monthlyStatistics = query
                .Where(hd => hd.NgayLapPhieu.HasValue)
                .GroupBy(hd => new { Year = hd.NgayLapPhieu.Value.Year, Month = hd.NgayLapPhieu.Value.Month })
                .Select(g => new
                {
                    Date = $"{g.Key.Month:00}/{g.Key.Year}", // Định dạng tháng/năm
                    TotalRevenue = g.Sum(hd => hd.TongTienThu)
                })
                .ToList();

            return View(monthlyStatistics);
        }
        [Route("RevenueStatisticsYear")]
        public IActionResult RevenueStatisticsYear(DateOnly? fromYear, DateOnly? toYear)
        {
            var query = db.HoaDons.AsQueryable();

            if (fromYear.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu >= fromYear.Value);
            }

            if (toYear.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu <= toYear.Value);
            }

            var yearlyStatistics = query
                .Where(hd => hd.NgayLapPhieu.HasValue)
                .GroupBy(hd => hd.NgayLapPhieu.Value.Year)
                .Select(g => new
                {
                    Date = g.Key.ToString(),
                    TotalRevenue = g.Sum(hd => hd.TongTienThu)
                })
                .ToList();

            return View(yearlyStatistics);
        }
    }
}
