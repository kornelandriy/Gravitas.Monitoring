using System.ComponentModel.DataAnnotations;

namespace Gravitas.Monitoring.Models
{
	public class Tickets
	{
		public int Id { get; set; }
		public int TicketContainerId { get; set; }
		public int StatusId { get; set; }
		public int OrderNo { get; set; }
		public int? SecondaryRouteTemplateId { get; set; }
		public int SecondaryRouteItemIndex { get; set; }
		public int? RouteTemplateId { get; set; }
		public int RouteItemIndex { get; set; }
		public int RouteType { get; set; }
		public int? NodeId { get; set; }
		public int Cycles { get; set; }
	}
}