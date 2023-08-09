.PHONY: dev
dev:
	dotnet run --project Cine-Plus-Api

.PHONY: restore
restore:
	dotnet restore

.PHONY: db
db:
	dotnet dotnet ef database update --project Cine-Plus-Api

.PHONY: build
build:
	dotnet build