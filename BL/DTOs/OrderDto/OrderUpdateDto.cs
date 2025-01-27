using ZelnyTrh.EF.DAL.Enums;

namespace ZelnyTrh.EF.BL.DTOs.OrderDto;

public class OrderUpdateDto
{
    public OrderStatus OrderStatus { get; set; }
}
