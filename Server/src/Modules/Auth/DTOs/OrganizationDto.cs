using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AuthModule.DTOs;

public class OrganizationDto
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("is_tanca_email")]
    public bool? IsTancaEmail { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("phone_code")]
    public string PhoneCode { get; set; }

    [JsonProperty("is_tanca_phone")]
    public bool? IsTancaPhone { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("birthday")]
    public string Birthday { get; set; }

    [JsonProperty("sex")]
    public int Sex { get; set; }

    [JsonProperty("identify_card")]
    public string IdentifyCard { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("is_root")]
    public int IsRoot { get; set; }

    [JsonProperty("birthplace")]
    public string Birthplace { get; set; }

    [JsonProperty("sub_department")]
    public string SubDepartment { get; set; }

    [JsonProperty("sub_team")]
    public string SubTeam { get; set; }

    [JsonProperty("nationality")]
    public string Nationality { get; set; }

    [JsonProperty("seniority_date")]
    public string SeniorityDate { get; set; }

    [JsonProperty("name_no_sign_shopee")]
    public string NameNoSignShopee { get; set; }

    [JsonProperty("direct_manager_id")]
    public string DirectManagerId { get; set; }

    [JsonProperty("alias")]
    public string Alias { get; set; }

    [JsonProperty("view_allowance_info")]
    public bool ViewAllowanceInfo { get; set; }

    [JsonProperty("view_promotion_history")]
    public bool ViewPromotionHistory { get; set; }

    [JsonProperty("disable_integration_menu")]
    public bool DisableIntegrationMenu { get; set; }

    [JsonProperty("can_update_permission")]
    public bool CanUpdatePermission { get; set; }

    [JsonProperty("can_update_timetracking_config")]
    public bool CanUpdateTimetrackingConfig { get; set; }

    [JsonProperty("assign_import_task_timesheet_setting")]
    public bool AssignImportTaskTimesheetSetting { get; set; }

    [JsonProperty("crud_task_timesheet_setting")]
    public bool CrudTaskTimesheetSetting { get; set; }

    [JsonProperty("can_use_publish_shifts")]
    public bool CanUsePublishShifts { get; set; }

    [JsonProperty("export_file_permission")]
    public ExportFilePermissionDto ExportFilePermission { get; set; }

    [JsonProperty("lock_timesheet_permission")]
    public int LockTimesheetPermission { get; set; }

    [JsonProperty("organization_id")]
    public string OrganizationId { get; set; }

    [JsonProperty("user_id")]
    public string UserId { get; set; }

    [JsonProperty("package")]
    public List<object> Package { get; set; }

    [JsonProperty("shop_id")]
    public string ShopId { get; set; }

    [JsonProperty("region_id")]
    public string RegionId { get; set; }

    [JsonProperty("branch_id")]
    public string BranchId { get; set; }

    [JsonProperty("position_id")]
    public string PositionId { get; set; }

    [JsonProperty("department_id")]
    public string DepartmentId { get; set; }

    [JsonProperty("group_id")]
    public string GroupId { get; set; }

    [JsonProperty("salary")]
    public decimal Salary { get; set; }

    [JsonProperty("tax")]
    public string Tax { get; set; }

    [JsonProperty("union")]
    public string Union { get; set; }

    [JsonProperty("client_role")]
    public string ClientRole { get; set; }

    [JsonProperty("identification")]
    public string Identification { get; set; }

    [JsonProperty("payroll_config_element_id")]
    public string PayrollConfigElementId { get; set; }

    [JsonProperty("payroll_config_id")]
    public string PayrollConfigId { get; set; }

    [JsonProperty("payroll_config_date")]
    public string PayrollConfigDate { get; set; }

    [JsonProperty("labour_end_date")]
    public string LabourEndDate { get; set; }

    [JsonProperty("working_date")]
    public string WorkingDate { get; set; }

    [JsonProperty("is_multi_mobile")]
    public int IsMultiMobile { get; set; }

    [JsonProperty("is_in_out_without_wifi")]
    public int IsInOutWithoutWifi { get; set; }

    [JsonProperty("is_no_need_timekeeping")]
    public int IsNoNeedTimekeeping { get; set; }

    [JsonProperty("is_auto_timekeeping")]
    public int IsAutoTimekeeping { get; set; }

    [JsonProperty("is_help_check_shift")]
    public int IsHelpCheckShift { get; set; }

    [JsonProperty("is_clock_in_using_image")]
    public int IsClockInUsingImage { get; set; }

    [JsonProperty("is_clock_out_using_image")]
    public int IsClockOutUsingImage { get; set; }

    [JsonProperty("is_location_tracking")]
    public int IsLocationTracking { get; set; }

    [JsonProperty("is_enable_auto_login")]
    public int IsEnableAutoLogin { get; set; }

    [JsonProperty("is_inactive")]
    public int IsInactive { get; set; }

    [JsonProperty("is_allowing_early_check_in_out")]
    public int IsAllowingEarlyCheckInOut { get; set; }

    [JsonProperty("is_allowing_lately_check_in_out")]
    public int IsAllowingLatelyCheckInOut { get; set; }

    [JsonProperty("is_help_check_shift_using_image")]
    public int IsHelpCheckShiftUsingImage { get; set; }

    [JsonProperty("is_auto_checkout")]
    public int IsAutoCheckout { get; set; }

    [JsonProperty("setting")]
    public OrganizationSettingDto Setting { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("is_head_of_department")]
    public int IsHeadOfDepartment { get; set; }

    [JsonProperty("is_quit")]
    public int IsQuit { get; set; }

    [JsonProperty("crm_noti_count")]
    public int CrmNotiCount { get; set; }

    [JsonProperty("sort_index")]
    public int SortIndex { get; set; }

    [JsonProperty("checkin_late")]
    public int CheckinLate { get; set; }

    [JsonProperty("checkout_early")]
    public int CheckoutEarly { get; set; }

    [JsonProperty("total_minutes_check_late_early")]
    public int TotalMinutesCheckLateEarly { get; set; }

    [JsonProperty("total_minutes_check_late_early_flag")]
    public int TotalMinutesCheckLateEarlyFlag { get; set; }

    [JsonProperty("checkin_late_ranges")]
    public List<object> CheckinLateRanges { get; set; }

    [JsonProperty("checkout_early_ranges")]
    public List<object> CheckoutEarlyRanges { get; set; }

    [JsonProperty("total_minutes_check_late_early_ranges")]
    public List<object> TotalMinutesCheckLateEarlyRanges { get; set; }

    [JsonProperty("shop_username")]
    public string ShopUsername { get; set; }

    [JsonProperty("position")]
    public string Position { get; set; }

    [JsonProperty("department")]
    public string Department { get; set; }

    [JsonProperty("shop")]
    public string Shop { get; set; }

    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("branch")]
    public string Branch { get; set; }

    [JsonProperty("payroll_config")]
    public string PayrollConfig { get; set; }

    [JsonProperty("group")]
    public string Group { get; set; }

    [JsonProperty("position_obj")]
    public object PositionObj { get; set; }

    [JsonProperty("department_obj")]
    public object DepartmentObj { get; set; }

    [JsonProperty("shop_obj")]
    public ShopObjDto ShopObj { get; set; }

    [JsonProperty("region_obj")]
    public object RegionObj { get; set; }

    [JsonProperty("branch_obj")]
    public object BranchObj { get; set; }

    [JsonProperty("payroll_config_obj")]
    public object PayrollConfigObj { get; set; }

    [JsonProperty("group_obj")]
    public GroupObjDto GroupObj { get; set; }

    [JsonProperty("user")]
    public UserDto User { get; set; }

    [JsonProperty("lock_timesheet_status")]
    public int LockTimesheetStatus { get; set; }

    [JsonProperty("shop_amount")]
    public int ShopAmount { get; set; }
}

public class ExportFilePermissionDto
{
    [JsonProperty("employee")]
    public int Employee { get; set; }
    [JsonProperty("insurance")]
    public int Insurance { get; set; }
    [JsonProperty("contract")]
    public int Contract { get; set; }
    [JsonProperty("asset")]
    public int Asset { get; set; }
    [JsonProperty("timesheet")]
    public int Timesheet { get; set; }
    [JsonProperty("time_tracking")]
    public int TimeTracking { get; set; }
    [JsonProperty("edit_time_tracking")]
    public int EditTimeTracking { get; set; }
    [JsonProperty("request")]
    public int Request { get; set; }
    [JsonProperty("payroll")]
    public int Payroll { get; set; }
    [JsonProperty("kpi")]
    public int Kpi { get; set; }
    [JsonProperty("task")]
    public int Task { get; set; }
    [JsonProperty("my_report")]
    public int MyReport { get; set; }
    [JsonProperty("request_approval")]
    public int RequestApproval { get; set; }
    [JsonProperty("gps")]
    public int Gps { get; set; }
    [JsonProperty("wifi")]
    public int Wifi { get; set; }
    [JsonProperty("qr")]
    public int Qr { get; set; }
    [JsonProperty("wanip")]
    public int Wanip { get; set; }
    [JsonProperty("project")]
    public int Project { get; set; }
    [JsonProperty("shift_list")]
    public int ShiftList { get; set; }
    [JsonProperty("who_is_working")]
    public int WhoIsWorking { get; set; }
    [JsonProperty("meal")]
    public int Meal { get; set; }
    [JsonProperty("employee_dayleft")]
    public int EmployeeDayleft { get; set; }
    [JsonProperty("promotion_history")]
    public int PromotionHistory { get; set; }
    [JsonProperty("timesheet_task")]
    public int TimesheetTask { get; set; }
}

public class OrganizationSettingDto
{
    [JsonProperty("auto_confirm_shift")]
    public int AutoConfirmShift { get; set; }
    [JsonProperty("clock_out_notification")]
    public int ClockOutNotification { get; set; }
}

public class ShopObjDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
}

public class GroupObjDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("client_role")]
    public string ClientRole { get; set; }
}

public class UserDto
{
    [JsonProperty("active")]
    public bool Active { get; set; }
    [JsonProperty("avatarOrigin")]
    public string AvatarOrigin { get; set; }
    [JsonProperty("birthday")]
    public object Birthday { get; set; }
    [JsonProperty("brandRepresent")]
    public object BrandRepresent { get; set; }
    [JsonProperty("company")]
    public List<object> Company { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("job")]
    public object Job { get; set; }
    [JsonProperty("language")]
    public string Language { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("sex")]
    public int Sex { get; set; }
    [JsonProperty("status")]
    public string Status { get; set; }
    [JsonProperty("statusConnection")]
    public string StatusConnection { get; set; }
    [JsonProperty("statusDefault")]
    public string StatusDefault { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("utcOffset")]
    public int UtcOffset { get; set; }
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("_version")]
    public int Version { get; set; }
    [JsonProperty("apiToken")]
    public string ApiToken { get; set; }
}