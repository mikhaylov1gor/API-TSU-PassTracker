namespace API_TSU_PassTracker.Models.DTO
{
    public class UserPagedListModel
    {
        public List<UserModel>? users { get; set; }
        public PageInfoModel pagination { get; set; }
    }
}
