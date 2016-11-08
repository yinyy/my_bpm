[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BPM.Admin.App_Start.Combres), "PreStart")]
namespace BPM.Admin.App_Start {
	using System.Web.Routing;
	using global::Combres;
	
    public static class Combres {
        public static void PreStart() {
            RouteTable.Routes.AddCombresRoute("Combres");
        }
    }
}