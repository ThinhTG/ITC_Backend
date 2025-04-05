namespace ITC.Core.Constants
{
    public static class EndPointAPI
    {
        public const string AreaName = "api";
        public const string AreaNameV2 = "apiV2";
        public static class Auth
        {
            private const string BaseEndpoint = "~/" + AreaName + "/auth";
            public const string LoginEndPoint = BaseEndpoint + "/login";
            public const string Info = BaseEndpoint + "/info";
            public const string AccessToken = BaseEndpoint + "/accesstoken";

        }
        public static class Roles
        {
           
            private const string BaseEndpoint = "~/" + AreaName + "/roles";
            private const string IdSegment = "/{id}";
            public const string Role = BaseEndpoint;
            public const string RoleById = BaseEndpoint + IdSegment;
        }

        public static class Ips
        {
            private const string BaseEndpoint = "~/" + AreaName + "/ips";
            private const string IdSegment = "/{id}";
            public const string Ip = BaseEndpoint;
            public const string IdIp = BaseEndpoint + IdSegment;
        }

        public static class Recaptcha
        {
            private const string BaseEndpoint = "~/" + AreaName + "/Recaptcha";
            public const string Captcha = BaseEndpoint;
        }

        public static class RoleClaims
        {
            public const string RoleClaim = "~/" + AreaName + "/roleclaims"; // endpoint cho lấy tất cả role claims
            public const string RoleClaimById = RoleClaim + "/{id}"; // Định nghĩa endpoint cho việc lấy RoleClaim theo ID
            public const string GetAll = "roleclaims"; // Endpoint cho việc lấy tất cả RoleClaims
            public const string Create = "roleclaims"; // Endpoint cho việc tạo RoleClaim
            public const string Update = "roleclaims/{id}"; // Endpoint cho việc cập nhật RoleClaim
            public const string Delete = "roleclaims/{id}"; // Endpoint cho việc xóa RoleClaim
        }
        public static class KeyEndPoints
        {
            public const string KeyEndpoint = "~/" + AreaName + "/keyendpoints";
            public const string KeyEndpointId = KeyEndpoint + "/{id}";
        }
        public static class KeyPairs
        {
            public const string KeyPair = "~/" + AreaName + "/keypairs";
            public const string KeyPairId = KeyPair + "/{id}";
        }
        public static class Order
        {
            private const string BaseEndpoint = "~/" + AreaName + "/orderproducts";

            public const string OrderProduct = BaseEndpoint;
            public const string OrderProductId = BaseEndpoint + "/{id}";
            public const string MergeOrder = BaseEndpoint + "/mergeorder";
        }

        public static class QAProduct
        {
            public const string BaseEndpoint = "~/" + AreaName + "/qaproducts";

            public const string QAProductsOfOrders = BaseEndpoint + "/{orderProductId}/createqaproduct";

            public const string TaskAndOrder = BaseEndpoint + "/gettaskandorder";

            public const string QAProductId = BaseEndpoint + "/{id}";

            public const string QuantityProduct = BaseEndpoint + "/quantityProduct";

        }

        public static class Warehouse
        {
            private const string BaseEndpoint = "~/" + AreaName + "/warehouse-management";

            public const string WareHouse = BaseEndpoint + "/WareHouse";
            public const string Line = BaseEndpoint + "/lineproducts";
            public const string LineId = BaseEndpoint + "/lineproducts/{id}";

            public const string Floor = BaseEndpoint + "/floorproducts";
            public const string FloorId = BaseEndpoint + "/floorproducts/{id}";

            public const string Chamber = BaseEndpoint + "/chambers";
            public const string ChamberId = BaseEndpoint + "/chambers/{id}";

            public const string ExportGeneral = BaseEndpoint + "/ExportGeneral";
            public const string ExportItem = BaseEndpoint + "/ExportItem";
            public const string ChamberTotal = BaseEndpoint + "/ChamberTotal";
            public const string ExportTrace = BaseEndpoint + "/export/trace";
            public const string TaskProductInMonth = BaseEndpoint + "/export/TaskProductInMonth";
        }

        public static class Users
        {
            public const string BaseEndPoint = "~/" + AreaName + "/users";
            public const string Code = BaseEndPoint;
            public const string Role = BaseEndPoint + "/role";
            public const string RoleName = Role + "/{roleName}";
            public const string UserId = BaseEndPoint + "/{id}";
            public const string ChangePassword = BaseEndPoint + "/changepassword/{id}";
        }
      
        public static class Modules
        {
            public const string BaseEndPoint = "~/" + AreaName + "/modules";
            public const string WebEndPoint = "~/" + AreaName + "/modules/web";
            public const string MobileEndPoint = "~/" + AreaName + "/modules/mobile";
			public const string Code = BaseEndPoint;
            public const string UpdateModule = BaseEndPoint + "/{id}";
            public const string ModuleId = BaseEndPoint + "/{id}";
            public const string Mobile = BaseEndPoint + "/mobile";
        }
      
        public static class DashBoards
        {
            public const string BaseEndPoint = "~/" + AreaName + "/dashboards";
            public const string Code = BaseEndPoint;
            public const string Owe = BaseEndPoint + "/getowe";
            public const string OweTv = BaseEndPoint + "/getowetivi";
            public const string Productivity = BaseEndPoint + "/getproductivity";
            public const string ProductivityTv = BaseEndPoint + "/getproductivitytivi";
            public const string Quality = BaseEndPoint + "/getquality";
            public const string QualityTv = BaseEndPoint + "/getqualitytivi";
            public const string KpiDefectCode = BaseEndPoint + "/getkpidefectcode";
            public const string DailyDefectCodePercent = BaseEndPoint + "/getdailydefectcodepercent";
            public const string DailyDefectCodePercentTv = BaseEndPoint + "/getdailydefectcodepercenttivi";
            public const string HourlyDefectCodePercent = BaseEndPoint + "/gethourlydefectcodepercent";
            public const string QuantityModuleChart = BaseEndPoint + "/quantity-module-chart";
            public const string StackQualityQuantity = BaseEndPoint + "/getstackqualityquantity";
            public const string HourHasDefectProduct = BaseEndPoint + "/gethourhasdefectproduct";
            public const string ModuleDashboardChart = BaseEndPoint + "/getmoduledashboardchart";

        }

        public static class TaskProduct
        {
            private const string BaseEndpoint = "~/" + AreaName + "/TaskProduct";
            public const string GetTaskProduct = BaseEndpoint + "s";
            public const string GetDayTaskProduct = BaseEndpoint + "/getDaytaskProduct";
            public const string TaskProductId = BaseEndpoint + "/{id}";
            public const string TaskProductApi = BaseEndpoint;
            public const string TaskByModuleAndDate = BaseEndpoint + "/moduleanddate";
            public const string TaskMerger = BaseEndpoint + "/taskmerger";
            public const string UpNumberOfWorksApi = BaseEndpoint + "/upNumberOfWorks" ;

        }

        public static class TaskDetail
        {
            private const string BaseEndpoint = "~/" + AreaName + "/taskdetail";
            public const string TaskDetailApi = BaseEndpoint;
            public const string TaskDetailId = BaseEndpoint + "/{id}";
            public const string TaskDetailTaskId = BaseEndpoint + "s/{id}";
            public const string GetTaskDetail = BaseEndpoint + "/{taskProductId}";
            public const string UpQuantityTaskDetail = BaseEndpoint + "/upQuantity";
        }

        public static class SlotTime
        {
            private const string BaseEndpoint = "~/" + AreaName + "/slottime";

            public const string Slottime = BaseEndpoint;
            public const string SlottimeId = BaseEndpoint + "/{id}";
        }
      
        public static class DefectCodeAPI
        {
            private const string BaseEndpoint = "~/" + AreaName + "/defectcodes";
            public const string Code = BaseEndpoint;
            public const string DefectCodeId = BaseEndpoint + "/{id}";

        }
        public static class DefectProductAPI
        {
            private const string BaseEndpoint = "~/" + AreaName + "/defectproducts";
            public const string DefectProduct = BaseEndpoint;
            public const string DefectProductId = BaseEndpoint + "/{id}";
            public const string OrderProductId = BaseEndpoint + "/{orderProductId}" + "/getqadefectproduct";


        }

        public static class Categories
        {
            private const string BaseEndpoint = "~/" + AreaName + "/categories";
            public const string Categorie = BaseEndpoint;
            public const string CategoriesId = BaseEndpoint + "/{id}";
        }

        public static class Products
        {
            private const string BaseEndpoint = "~/" + AreaName + "/products";
            public const string Product = BaseEndpoint;
            public const string ProductsId = BaseEndpoint + "/{id}";

        }

        public static class PalletProduct
        {
            private const string BaseEndpoint = "~/" + AreaName + "/palletproducts";
            public const string Palletproduct = BaseEndpoint;
            public const string PalletProductId = BaseEndpoint + "/{id}";
            public const string PalletQR = BaseEndpoint + "/QR";
        }

        public static class BoxProducts
        {
            private const string BaseEndpoint = "~/" + AreaName + "/boxproducts";
            public const string Boxproducts = BaseEndpoint;
            public const string BoxProductCode = BaseEndpoint + "/{code}";
        }

        public static class UserClaims
        {
            private const string BaseEndpoint = "~/" + AreaName + "/userclaims";
            public const string UserClaim = BaseEndpoint;
            public const string UserClaimId = BaseEndpoint + "/{id}";
        }

        public static class InventoryHistory
        {
            private const string BaseEndpoint = "~/" + AreaName;
            public const string HisChamber = BaseEndpoint + "/historyChamber";
            public const string HisPallet = BaseEndpoint + "/historyPallet";
            public const string HisBox = BaseEndpoint + "/historyBox";

        }
    }
   
}
