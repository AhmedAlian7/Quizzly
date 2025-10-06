namespace Quizzly.DataAccess.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Instructor = "Instructor";
        public const string Student = "Student";
        public static readonly string[] All = { Admin, Instructor, Student };
    }
}
