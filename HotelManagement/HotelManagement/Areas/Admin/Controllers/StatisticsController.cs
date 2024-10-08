using HotelManagement.Filters;
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
        [Authentication]
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
        public IActionResult RevenueStatisticsYear(int? fromYear, int? toYear)
        {
            var query = db.HoaDons.AsQueryable();

            if (fromYear.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu.Value.Year >= fromYear.Value);
            }

            if (toYear.HasValue)
            {
                query = query.Where(hd => hd.NgayLapPhieu.Value.Year <= toYear.Value);
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

        [Route("RoomUsageStatistics")]
        public IActionResult RoomUsageStatistics(DateOnly? fromDate, DateOnly? toDate)
        {
            // Khởi tạo truy vấn cho DatPhongDoan
            var groupBookingQuery = db.DatPhongDoans.AsQueryable();
            if (fromDate.HasValue)
            {
                groupBookingQuery = groupBookingQuery.Where(dp => dp.NgayDat >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                groupBookingQuery = groupBookingQuery.Where(dp => dp.NgayDat <= toDate.Value);
            }

            // Khởi tạo truy vấn cho DatPhong
            var individualBookingQuery = db.DatPhongs.AsQueryable();
            if (fromDate.HasValue)
            {
                individualBookingQuery = individualBookingQuery.Where(dp => dp.NgayNhan >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                individualBookingQuery = individualBookingQuery.Where(dp => dp.NgayNhan <= toDate.Value);
            }

            var groupBookingData = groupBookingQuery
             .SelectMany(dp => dp.DatPhongDoanPhongs.Select(dpd => new { NgayDat = dp.NgayDat, RoomId = dpd.MaPhong }));

            var individualBookingData = individualBookingQuery
                .Select(dp => new { NgayDat = dp.NgayNhan, RoomId = dp.MaPhong });

            // Kết hợp dữ liệu
            var result = groupBookingData
                .Concat(individualBookingData)
                .GroupBy(x => x.NgayDat)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRoomsUsed = g.Select(x => x.RoomId).Count()
                })
                .ToList();

            return View(result);
        }
        [Route("RoomUsageStatisticsMonth")]
        public IActionResult RoomUsageStatisticsMonth(DateOnly? fromMonth, DateOnly? toMonth)
        {
            // Khởi tạo truy vấn cho DatPhongDoan
            var groupBookingQuery = db.DatPhongDoans.AsQueryable();
            if (fromMonth.HasValue)
            {
                groupBookingQuery = groupBookingQuery.Where(dp => dp.NgayDat >= fromMonth.Value);
            }
            if (toMonth.HasValue)
            {
                groupBookingQuery = groupBookingQuery.Where(dp => dp.NgayDat <= toMonth.Value);
            }

            // Khởi tạo truy vấn cho DatPhong
            var individualBookingQuery = db.DatPhongs.AsQueryable();
            if (fromMonth.HasValue)
            {
                individualBookingQuery = individualBookingQuery.Where(dp => dp.NgayNhan >= fromMonth.Value);
            }
            if (toMonth.HasValue)
            {
                individualBookingQuery = individualBookingQuery.Where(dp => dp.NgayNhan <= toMonth.Value);
            }

            var groupBookingData = groupBookingQuery
                .SelectMany(dp => dp.DatPhongDoanPhongs.Select(dpd => new
                {
                    Year = dp.NgayDat.Value.Year,
                    Month = dp.NgayDat.Value.Month,
                    RoomId = dpd.MaPhong
                }));

            var individualBookingData = individualBookingQuery
                .Select(dp => new
                {
                    Year = dp.NgayNhan.Value.Year,
                    Month = dp.NgayNhan.Value.Month,
                    RoomId = dp.MaPhong
                });

            // Kết hợp dữ liệu
            var result = groupBookingData
                .Concat(individualBookingData)
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new
                {
                    Date = $"{g.Key.Month:00}/{g.Key.Year}", // Định dạng tháng/năm
                    TotalRoomsUsed = g.Select(x => x.RoomId).Count()
                })
                .ToList();

            return View(result);
        }

        [Route("RoomUsageStatisticsYear")]
        public IActionResult RoomUsageStatisticsYear(int? fromYear, int? toYear)
        {
            var queryDatPhongDoan = db.DatPhongDoans.AsQueryable();

            if (fromYear.HasValue)
            {
                queryDatPhongDoan = queryDatPhongDoan.Where(dp => dp.NgayDat.Value.Year >= fromYear.Value);
            }

            if (toYear.HasValue)
            {
                queryDatPhongDoan = queryDatPhongDoan.Where(dp => dp.NgayDat.Value.Year <= toYear.Value);
            }

            var queryDatPhong = db.DatPhongs.AsQueryable();

            if (fromYear.HasValue)
            {
                queryDatPhong = queryDatPhong.Where(dp => dp.NgayNhan.Value.Year >= fromYear.Value);
            }

            if (toYear.HasValue)
            {
                queryDatPhong = queryDatPhong.Where(dp => dp.NgayNhan.Value.Year <= toYear.Value);
            }

            // Kết hợp dữ liệu từ cả hai bảng
            var yearlyStatistics = queryDatPhongDoan
                .SelectMany(dp => dp.DatPhongDoanPhongs.Select(dpd => new
                {
                    Year = dp.NgayDat.Value.Year,
                    RoomId = dpd.MaPhong
                }))
                .Concat(
                    queryDatPhong.Select(dp => new
                    {
                        Year = dp.NgayNhan.Value.Year,
                        RoomId = dp.MaPhong
                    })
                )
                .GroupBy(x => x.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    TotalRoomsUsed = g.Select(x => x.RoomId).Count() // Đếm số lượng phòng đã sử dụng
                })
                .ToList();

            return View(yearlyStatistics);
        }


    }
}
