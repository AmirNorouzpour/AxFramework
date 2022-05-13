namespace Common
{
    public enum AxOp
    {
        None,
        [AxDisplay("اطلاعات پایه", "01", None)]
        Basic,
        [AxDisplay("تصاویر", "01", UserItem)]
        PersonImages,
        [AxDisplay("درج", "01", PersonImages)]
        PersonImageInsert,
        [AxDisplay("حذف", "02", PersonImages)]
        PersonImageDelete,
        [AxDisplay("مشاهده", "03", PersonImages)]
        PersonImageShow,
        [AxDisplay("مدیریت کاربران", "02", Basic)]
        UserManagement,
        [AxDisplay("لیست کاربران", "01", UserManagement)]
        UserList,
        [AxDisplay("فرم کاربر", "02", UserManagement)]
        UserItem,
        [AxDisplay("ایجاد", "03", UserManagement)]
        UserInsert,
        [AxDisplay("ویرایش", "04", UserManagement)]
        UserUpdate,
        [AxDisplay("حذف کاربر", "05", UserManagement)]
        UserDelete,
        [AxDisplay("حقوق دسترسی", "11", UserManagement)]
        PermissionTree,
        [AxDisplay("ویرایش حقوق دسترسی", "11", PermissionTree)]
        PermissionTreeSave,
        [AxDisplay("لیست آدرس ها", "02", UserItem)]
        AddressList,
        [AxDisplay("فرم آدرس", "03", UserItem)]
        AddressItem,
        [AxDisplay("ایجاد", "04", AddressList)]
        AddressInsert,
        [AxDisplay("ویرایش", "05", AddressList)]
        AddressUpdate,
        [AxDisplay("حذف", "06", AddressList)]
        AddressDelete,
        [AxDisplay("داشبورد", "03", Basic)]
        DashboardIndex,
        [AxDisplay("رویدادنگاری", "01", Basic)]
        Audits,
        [AxDisplay("لیست خطا ها", "01", Audits)]
        LogList,
        [AxDisplay("جزئیات خطا", "01", LogList)]
        LogItem,
        [AxDisplay("تاریخچه تغییرات", "02", Audits)]
        ChangeTracker,
        [AxDisplay("جزئیات تاریخچه تغییرات", "01", ChangeTracker)]
        ChangeTrackerItem,

        [AxDisplay("گزارشات", "99", None)]
        Reports,
        [AxDisplay("گزارشات اطلاعات پایه", "01", Reports)]
        BasicSystemReports,
        //[AxDisplay("گزارشات اشخاص", "01", BasicSystemReports)]
        //PersonReports,
        [AxDisplay("گزارش لیست کاربران", "01", BasicSystemReports)]
        UserListReport,
        [AxDisplay("گزارش ورود های ناموفق", "02", BasicSystemReports)]
        UnSuccessfullyLoginReport,
        [AxDisplay("مدیریت حقوق دسترسی", "04", Basic)]
        AuthorizationManagement,
        [AxDisplay("حقوق دسترسی عملیات ها", "01", AuthorizationManagement)]
        OperationsAuthorization,
        [AxDisplay("لیست گروه ها", "06", UserManagement)]
        GroupList,
        [AxDisplay("فرم گروه", "07", GroupList)]
        GroupItem,
        [AxDisplay("ایجاد", "08", GroupList)]
        GroupInsert,
        [AxDisplay("ویرایش", "09", GroupList)]
        GroupUpdate,
        [AxDisplay("حذف گروه", "10", GroupList)]
        GroupDelete,
        [AxDisplay("اطلاعات عمومی", "05", Basic)]
        GeneralInfo,
        [AxDisplay("اطلاعات جغرافیایی", "01", GeneralInfo)]
        GeoInfo,
        [AxDisplay("ایجاد", "01", GeoInfo)]
        GeoInfoInsert,
        [AxDisplay("ویرایش", "02", GeoInfo)]
        GeoInfoUpdate,
        [AxDisplay("حذف", "03", GeoInfo)]
        GeoInfoDelete,


        //==================================================================================
        [AxDisplay("سامانه ردیابی محصول", "02", None)]
        Tracking,
        [AxDisplay("اطلاعات پایه", "01", Tracking)]
        TrackingBasicInfo,
        [AxDisplay("کارخانه ها", "01", TrackingBasicInfo)]
        TrackingFactoryList,
        [AxDisplay("ایجاد", "01", TrackingFactoryList)]
        FactoryInsert,
        [AxDisplay("ویرایش", "02", TrackingFactoryList)]
        FactoryUpdate,
        [AxDisplay("حذف", "03", TrackingFactoryList)]
        FactoryDelete,
        [AxDisplay("مشاهده", "04", TrackingFactoryList)]
        FactoryItem,
        [AxDisplay("خط های تولید", "02", TrackingBasicInfo)]
        ProductLineList,
        [AxDisplay("ایجاد", "01", ProductLineList)]
        ProductLineInsert,
        [AxDisplay("ویرایش", "02", ProductLineList)]
        ProductLineUpdate,
        [AxDisplay("حذف", "03", ProductLineList)]
        ProductLineDelete,
        [AxDisplay("مشاهده", "04", ProductLineList)]
        ProductLineItem,
        [AxDisplay("ماشین ها", "03", TrackingBasicInfo)]
        MachineList,
        [AxDisplay("ایجاد", "01", MachineList)]
        MachineInsert,
        [AxDisplay("ویرایش", "02", MachineList)]
        MachineUpdate,
        [AxDisplay("حذف", "03", MachineList)]
        MachineDelete,
        [AxDisplay("مشاهده", "04", MachineList)]
        MachineItem,
        [AxDisplay("ایستگاه ها", "04", TrackingBasicInfo)]
        OperationStationList,
        [AxDisplay("ایجاد", "01", OperationStationList)]
        OperationStationInsert,
        [AxDisplay("ویرایش", "02", OperationStationList)]
        OperationStationUpdate,
        [AxDisplay("حذف", "03", OperationStationList)]
        OperationStationDelete,
        [AxDisplay("مشاهده", "04", OperationStationList)]
        OperationStationItem,
        [AxDisplay("شیفت ها", "05", TrackingBasicInfo)]
        ShiftList,
        [AxDisplay("ایجاد", "01", ShiftList)]
        ShiftInsert,
        [AxDisplay("ویرایش", "02", ShiftList)]
        ShiftUpdate,
        [AxDisplay("حذف", "03", ShiftList)]
        ShiftDelete,
        [AxDisplay("مشاهده", "04", ShiftList)]
        ShiftItem,
        [AxDisplay("پرسنل", "06", TrackingBasicInfo)]
        PersonnelList,
        [AxDisplay("ایجاد", "01", PersonnelList)]
        PersonnelInsert,
        [AxDisplay("ویرایش", "02", PersonnelList)]
        PersonnelUpdate,
        [AxDisplay("حذف", "03", PersonnelList)]
        PersonnelDelete,
        [AxDisplay("مشاهده", "04", PersonnelList)]
        PersonnelItem,
        //=====================
        [AxDisplay("اطلاعات ردیابی", "02", Tracking)]
        TrackingInfo,
        [AxDisplay("لیست نمونه محصولات", "01", TrackingInfo)]
        ProductInstanceList,
        [AxDisplay("ایجاد", "01", ProductInstanceList)]
        ProductInstanceInsert,
        [AxDisplay("حذف", "02", ProductInstanceList)]
        ProductInstanceDelete,
        [AxDisplay("ویرایش", "03", ProductInstanceList)]
        ProductInstanceUpdate,
        [AxDisplay("مشاهده", "04", ProductInstanceList)]
        ProductInstanceItem,
        //=====================
        [AxDisplay("لیست تاریخچه ردیابی محصولات", "02", TrackingInfo)]
        ProductInstanceHistoryList,
        [AxDisplay("ایجاد", "01", ProductInstanceHistoryList)]
        ProductInstanceHistoryInsert,
        [AxDisplay("حذف", "02", ProductInstanceHistoryList)]
        ProductInstanceHistoryDelete,
        //=====================
        [AxDisplay("لیست ضایعات", "03", TrackingInfo)]
        DamagedList,
        [AxDisplay("ایجاد", "01", DamagedList)]
        DamagedListInsert,
        [AxDisplay("حذف", "02", DamagedList)]
        DamagedListDelete,
        //=====================
        [AxDisplay("لیست توقفات", "04", TrackingInfo)]
        StopList,
    }
}
