PROJECT_PATH = ./src/webApi
MIGRATIONS_PATH = Data/Migrations

.PHONY: run watch build clean restore \
		migration-add migration-remove migration-apply \
		db-drop docker-up docker-down

# ── App ──────────────────────────────────────────────────────────────────
run:
	dotnet run --project $(PROJECT_PATH)

watch:
	dotnet watch --project $(PROJECT_PATH)

build:
	dotnet build $(PROJECT_PATH)

clean:
	dotnet clean $(PROJECT_PATH)

restore:
	dotnet restore $(PROJECT_PATH)

add-package:
	dotnet add package $(name) --project $(PROJECT_PATH)


# ── Migrations ───────────────────────────────────────────────────────────
# Usage: make migration-add name=InitialCreate
migration-add:
	dotnet ef migrations add $(name) --project $(PROJECT_PATH) -o $(MIGRATIONS_PATH)

migration-remove:
	dotnet ef migrations remove --project $(PROJECT_PATH)

migration-apply:
	dotnet ef database update --project $(PROJECT_PATH)

db-drop:
	dotnet ef database drop --project $(PROJECT_PATH)

# ── Docker ───────────────────────────────────────────────────────────────
docker-up:
	docker-compose up -d

docker-down:
	docker-compose down

docker-logs:
	docker-compose logs -f

# ── Helpers ──────────────────────────────────────────────────────────────
help:
	@echo ""
	@echo "  make run                  Run the app"
	@echo "  make watch                Run with hot reload"
	@echo "  make build                Build the project"
	@echo "  make clean                Clean build artifacts"
	@echo "  make restore              Restore NuGet packages"
	@echo ""
	@echo "  make migration-add name=  Add a new migration"
	@echo "  make migration-remove     Remove last migration"
	@echo "  make migration-apply      Apply migrations to DB"
	@echo "  make db-drop              Drop the database"
	@echo ""
	@echo "  make docker-up            Start containers"
	@echo "  make docker-down          Stop containers"
	@echo "  make docker-logs          Tail container logs"
	@echo ""