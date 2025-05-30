using System;
using System.Collections.Generic;

namespace AuthModule.DTOs;

public class CompanyInfoDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Username { get; set; }
    public double? AddressLat { get; set; }
    public double? AddressLng { get; set; }
    public string Tel { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public List<string> Package { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Website { get; set; }
    public string Email { get; set; }
    public string TypeOfBusiness { get; set; }
    public string TaxCode { get; set; }
    public string BankAccount { get; set; }
    public string Bank { get; set; }
    public DateTime? FoundedDate { get; set; }
    public string Scale { get; set; }
    public string CharterCapital { get; set; }
    public string Logo { get; set; }
    public string BrandLogo { get; set; }
    public string Code { get; set; }
    public string Phone { get; set; }
    public int RGold { get; set; }
    public int YGold { get; set; }
    public bool IsShopee { get; set; }
    public int? RecruitmentTemplateId { get; set; }
    public string EmployeeExcelVersion { get; set; }
    public TutorialDto Tutorial { get; set; }
    public int GetStartedStep { get; set; }
    public List<string> NeedFieldUpdate { get; set; }
    public PackagesDto Packages { get; set; }
    public List<object> Surveys { get; set; }
    public bool IsUsingCameraAi { get; set; }
    public string TimeFormat { get; set; }
    public string DateFormat { get; set; }
    public int NeedVerifyPhone { get; set; }
    public bool IsUsingOnleaveV2 { get; set; }
    public List<SetupStepDto> SetupSteps { get; set; }
    public IntegrationDto Integration { get; set; }
    public bool TalentManagement { get; set; }
    public bool ElearningManagement { get; set; }
}

public class TutorialDto
{
    public int C1 { get; set; }
    public int C2 { get; set; }
    public int C3 { get; set; }
}

public class PackagesDto
{
    public PackageInfoDto Info { get; set; }
    public Dictionary<string, PackageDetailDto> Packages { get; set; }
}

public class PackageInfoDto
{
    public DateTime StartPackage { get; set; }
    public DateTime EndPackage { get; set; }
    public int Users { get; set; }
    public int ActiveUsers { get; set; }
    public int IsTrial { get; set; }
    public int IsFull { get; set; }
    public int Price { get; set; }
}

public class PackageDetailDto
{
    public string ShopId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public List<string> Roles { get; set; }
    public int Users { get; set; }
    public DateTime StartPackage { get; set; }
    public DateTime EndPackage { get; set; }
    public int IsTrial { get; set; }
    public LocalizedNameDto En { get; set; }
    public LocalizedNameDto Ja { get; set; }
    public int IsAvailable { get; set; }
}

public class LocalizedNameDto
{
    public string Name { get; set; }
}

public class SetupStepDto
{
    public string Code { get; set; }
    public int Weight { get; set; }
    public int IsDone { get; set; }
}

public class IntegrationDto
{
    public bool DigitalSignature { get; set; }
}