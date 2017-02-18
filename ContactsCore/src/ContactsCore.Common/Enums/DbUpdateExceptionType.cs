namespace ContactsCore.Common.Enums
{
    public enum DbUpdateExceptionType
    {
        Unknown = 0,
        ForeignKeyConstraintViolation = 1, 
        UniqueKeyConstraintViolation = 2,        
    }
}
