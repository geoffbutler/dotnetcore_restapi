from api dir:
dotnet restore

# GENERATE INITIAL:
dotnet ef --project ../ContactsCore.Data --startup-project . migrations add initial

# REMOVE LAST ADDED MIGRATION (MUST BE ROLLED BACK FIRST:
dotnet ef --project ../ContactsCore.Data --startup-project . migrations remove

# UPDATE DB TO LATEST
dotnet ef --project ../ContactsCore.Data --startup-project . database update

# ROLLBACK DB TO NOTHING
dotnet ef --project ../ContactsCore.Data --startup-project . database update 0

