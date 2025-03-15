namespace API_TSU_PassTracker.Models.DTO
{
    public class LightRequestsPagedListModel
    {
        public ListLightRequestsDTO requests { get; set; }
        public PageInfoModel pagination {  get; set; }
    }
}
