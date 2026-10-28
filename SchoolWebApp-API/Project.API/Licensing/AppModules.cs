namespace SchoolWebApp.API.Licensing
{
    /// <summary>
    /// The modules a school can buy. Core (dashboard, school, students, staff,
    /// settings, reports, security) comes with every licence and is not listed.
    /// The names are what licences carry, so never rename one.
    /// </summary>
    public static class AppModules
    {
        /// <summary>Academics (assessments, exams, learner development) and Teaching Management.</summary>
        public const string Academics = "Academics";
        public const string Finance = "Finance";
        public const string Payroll = "Payroll";
        public const string Elections = "Elections";
        /// <summary>Add-on: SMS and messaging, used by the other modules too.</summary>
        public const string Communications = "Communications";

        public static readonly string[] All = { Academics, Finance, Payroll, Elections, Communications };
    }
}
