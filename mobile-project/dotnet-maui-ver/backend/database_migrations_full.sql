CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "users" (
    "Id" uuid NOT NULL CONSTRAINT "PK_users" PRIMARY KEY,
    "FullName" character varying(100) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "PasswordHash" text NOT NULL,
    "BaseCurrency" character varying(10) NOT NULL DEFAULT 'VND',
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

CREATE TABLE "refresh_tokens" (
    "Id" uuid NOT NULL CONSTRAINT "PK_refresh_tokens" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Token" character varying(255) NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "RevokedAt" timestamp with time zone NULL,
    "ReplacedByToken" character varying(255) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_refresh_tokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_refresh_tokens_Token" ON "refresh_tokens" ("Token");

CREATE INDEX "IX_refresh_tokens_UserId" ON "refresh_tokens" ("UserId");

CREATE UNIQUE INDEX "IX_users_Email" ON "users" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022055855_InitialCreate_UsersAndAuth', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
ALTER TABLE "users" ADD "DeletedAt" timestamp with time zone NULL;

ALTER TABLE "users" ADD "IsDeleted" boolean NOT NULL DEFAULT 0;

ALTER TABLE "refresh_tokens" ADD "DeletedAt" timestamp with time zone NULL;

ALTER TABLE "refresh_tokens" ADD "IsDeleted" boolean NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022062457_AddSoftDeleteAuditFields', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "wallets" (
    "Id" uuid NOT NULL CONSTRAINT "PK_wallets" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "WalletType" integer NOT NULL,
    "Balance" numeric(18,2) NOT NULL,
    "InitialBalance" numeric(18,2) NOT NULL,
    "Currency" character varying(10) NOT NULL DEFAULT 'VND',
    "AccountNumber" character varying(50) NULL,
    "BankName" character varying(100) NULL,
    "CreditLimit" numeric(18,2) NULL,
    "StatementDay" integer NULL,
    "PaymentDueDay" integer NULL,
    "ColorHex" character varying(20) NOT NULL DEFAULT '#0066FF',
    "IconKey" character varying(50) NOT NULL DEFAULT 'wallet',
    "DisplayOrder" integer NOT NULL DEFAULT 0,
    "IsExcludedFromTotal" boolean NOT NULL DEFAULT 0,
    "Status" integer NOT NULL,
    "Version" integer NOT NULL DEFAULT 1,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_wallets_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "balance_adjustments" (
    "Id" uuid NOT NULL CONSTRAINT "PK_balance_adjustments" PRIMARY KEY,
    "WalletId" uuid NOT NULL,
    "OldBalance" numeric(18,2) NOT NULL,
    "NewBalance" numeric(18,2) NOT NULL,
    "DifferenceAmount" numeric(18,2) NOT NULL,
    "Reason" character varying(255) NULL,
    "AdjustmentDate" timestamp with time zone NOT NULL,
    "ProofImageUrl" character varying(500) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_balance_adjustments_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_balance_adjustments_WalletId_AdjustmentDate" ON "balance_adjustments" ("WalletId", "AdjustmentDate");

CREATE INDEX "IX_wallets_UserId_Status" ON "wallets" ("UserId", "Status");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022064110_AddWalletsAndBalanceAdjustments', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "categories" (
    "Id" uuid NOT NULL CONSTRAINT "PK_categories" PRIMARY KEY,
    "UserId" uuid NULL,
    "Name" character varying(100) NOT NULL,
    "TransactionType" integer NOT NULL,
    "ParentId" uuid NULL,
    "IconKey" character varying(50) NOT NULL DEFAULT 'folder',
    "ColorHex" character varying(20) NOT NULL DEFAULT '#10B981',
    "DisplayOrder" integer NOT NULL DEFAULT 0,
    "IsSystem" boolean NOT NULL DEFAULT 0,
    "IncomeGroup" integer NULL,
    "BudgetRuleMapping" integer NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_categories_categories_ParentId" FOREIGN KEY ("ParentId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_categories_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "transactions" (
    "Id" uuid NOT NULL CONSTRAINT "PK_transactions" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "TransactionType" integer NOT NULL,
    "TransactionDate" timestamp with time zone NOT NULL,
    "Note" character varying(500) NULL,
    "IsEssential" boolean NOT NULL DEFAULT 0,
    "IsWindfall" boolean NOT NULL DEFAULT 0,
    "IsRecurring" boolean NOT NULL DEFAULT 0,
    "ExcludeFromReport" boolean NOT NULL DEFAULT 0,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_transactions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_transactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_transactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_categories_IsSystem" ON "categories" ("IsSystem");

CREATE INDEX "IX_categories_ParentId" ON "categories" ("ParentId");

CREATE INDEX "IX_categories_UserId_TransactionType_IsDeleted" ON "categories" ("UserId", "TransactionType", "IsDeleted");

CREATE INDEX "IX_transactions_CategoryId_IsDeleted" ON "transactions" ("CategoryId", "IsDeleted");

CREATE INDEX "IX_transactions_UserId_TransactionDate_IsDeleted" ON "transactions" ("UserId", "TransactionDate", "IsDeleted");

CREATE INDEX "IX_transactions_UserId_TransactionType_TransactionDate" ON "transactions" ("UserId", "TransactionType", "TransactionDate");

CREATE INDEX "IX_transactions_WalletId_IsDeleted" ON "transactions" ("WalletId", "IsDeleted");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022135310_AddCategoriesAndTransactions', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "WalletTransfers" (
    "Id" uuid NOT NULL CONSTRAINT "PK_WalletTransfers" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "SourceWalletId" uuid NOT NULL,
    "DestinationWalletId" uuid NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "TransferDate" timestamp with time zone NOT NULL,
    "Note" character varying(500) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_WalletTransfers_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_WalletTransfers_wallets_DestinationWalletId" FOREIGN KEY ("DestinationWalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_WalletTransfers_wallets_SourceWalletId" FOREIGN KEY ("SourceWalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_WalletTransfers_DestinationWalletId" ON "WalletTransfers" ("DestinationWalletId");

CREATE INDEX "IX_WalletTransfers_SourceWalletId" ON "WalletTransfers" ("SourceWalletId");

CREATE INDEX "IX_WalletTransfers_UserId_TransferDate" ON "WalletTransfers" ("UserId", "TransferDate");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022144257_AddWalletTransfers', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "recurring_incomes" (
    "Id" uuid NOT NULL CONSTRAINT "PK_recurring_incomes" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "ExpectedAmount" numeric(18,2) NOT NULL,
    "WalletId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "Frequency" integer NOT NULL,
    "DayOfMonth" integer NOT NULL,
    "AutoRecord" boolean NOT NULL DEFAULT 1,
    "IsActive" boolean NOT NULL DEFAULT 1,
    "NextDueDate" timestamp with time zone NOT NULL,
    "LastGeneratedDate" timestamp with time zone NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_recurring_incomes_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recurring_incomes_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_recurring_incomes_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_recurring_incomes_CategoryId" ON "recurring_incomes" ("CategoryId");

CREATE INDEX "IX_recurring_incomes_IsActive_AutoRecord_NextDueDate" ON "recurring_incomes" ("IsActive", "AutoRecord", "NextDueDate");

CREATE INDEX "IX_recurring_incomes_UserId_IsDeleted" ON "recurring_incomes" ("UserId", "IsDeleted");

CREATE INDEX "IX_recurring_incomes_WalletId" ON "recurring_incomes" ("WalletId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022145303_AddRecurringIncomes', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "salary_slip_details" (
    "Id" uuid NOT NULL CONSTRAINT "PK_salary_slip_details" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "TransactionId" uuid NOT NULL,
    "EmployerName" character varying(200) NOT NULL,
    "PayPeriod" character varying(50) NOT NULL,
    "GrossSalary" numeric(18,2) NOT NULL,
    "NetSalary" numeric(18,2) NOT NULL,
    "SocialInsurance" numeric(18,2) NOT NULL,
    "HealthInsurance" numeric(18,2) NOT NULL,
    "UnemploymentInsurance" numeric(18,2) NOT NULL,
    "PersonalIncomeTax" numeric(18,2) NOT NULL,
    "OtherDeductions" numeric(18,2) NOT NULL DEFAULT '0.0',
    "Allowances" numeric(18,2) NOT NULL DEFAULT '0.0',
    "Bonus" numeric(18,2) NOT NULL DEFAULT '0.0',
    "IsMathAudited" boolean NOT NULL DEFAULT 0,
    "DiscrepancyAmount" numeric(18,2) NOT NULL DEFAULT '0.0',
    "RawOcrJson" text NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_salary_slip_details_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_salary_slip_details_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_salary_slip_details_TransactionId" ON "salary_slip_details" ("TransactionId");

CREATE INDEX "IX_salary_slip_details_UserId_PayPeriod" ON "salary_slip_details" ("UserId", "PayPeriod");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022150459_AddSalarySlipDetails', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "auto_categorization_rules" (
    "Id" uuid NOT NULL CONSTRAINT "PK_auto_categorization_rules" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "KeywordPattern" character varying(100) NOT NULL,
    "TargetCategoryId" uuid NOT NULL,
    "PriorityOrder" integer NOT NULL DEFAULT 0,
    "IsRegex" boolean NOT NULL DEFAULT 0,
    "IsCaseSensitive" boolean NOT NULL DEFAULT 0,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_auto_categorization_rules_categories_TargetCategoryId" FOREIGN KEY ("TargetCategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_auto_categorization_rules_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "receipt_items" (
    "Id" uuid NOT NULL CONSTRAINT "PK_receipt_items" PRIMARY KEY,
    "TransactionId" uuid NOT NULL,
    "ItemName" character varying(200) NOT NULL,
    "Quantity" numeric(18,3) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "TotalPrice" numeric(18,2) NOT NULL,
    "CategoryId" uuid NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_receipt_items_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_receipt_items_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_auto_categorization_rules_TargetCategoryId" ON "auto_categorization_rules" ("TargetCategoryId");

CREATE INDEX "IX_auto_categorization_rules_UserId_KeywordPattern" ON "auto_categorization_rules" ("UserId", "KeywordPattern");

CREATE INDEX "IX_auto_categorization_rules_UserId_PriorityOrder" ON "auto_categorization_rules" ("UserId", "PriorityOrder");

CREATE INDEX "IX_receipt_items_CategoryId" ON "receipt_items" ("CategoryId");

CREATE INDEX "IX_receipt_items_TransactionId" ON "receipt_items" ("TransactionId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022155322_AddExpenseReceiptAndAutoRules', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "budgets" (
    "Id" uuid NOT NULL CONSTRAINT "PK_budgets" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "PeriodType" integer NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "TotalAmount" numeric(18,2) NOT NULL,
    "Method" integer NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

CREATE TABLE "budget_allocations" (
    "Id" uuid NOT NULL CONSTRAINT "PK_budget_allocations" PRIMARY KEY,
    "BudgetId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "AllocatedAmount" numeric(18,2) NOT NULL,
    "CurrentSpent" numeric(18,2) NOT NULL,
    "AlertThresholdPercent" numeric(5,2) NOT NULL DEFAULT '80.0',
    CONSTRAINT "FK_budget_allocations_budgets_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "budgets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_budget_allocations_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "daily_spending_allowances" (
    "Id" uuid NOT NULL CONSTRAINT "PK_daily_spending_allowances" PRIMARY KEY,
    "BudgetId" uuid NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "DailyAllowanceAmount" numeric(18,2) NOT NULL,
    "ActualSpentToday" numeric(18,2) NOT NULL,
    "RemainingAllowanceToday" numeric(18,2) NOT NULL,
    "VelocityIndex" numeric(8,4) NOT NULL DEFAULT '1.0',
    "OutOfMoneyForecastDate" timestamp with time zone NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_daily_spending_allowances_budgets_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "budgets" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_budget_allocations_BudgetId_CategoryId" ON "budget_allocations" ("BudgetId", "CategoryId");

CREATE INDEX "IX_budget_allocations_CategoryId" ON "budget_allocations" ("CategoryId");

CREATE INDEX "IX_budgets_UserId_StartDate_EndDate" ON "budgets" ("UserId", "StartDate", "EndDate");

CREATE INDEX "IX_budgets_UserId_Status" ON "budgets" ("UserId", "Status");

CREATE UNIQUE INDEX "IX_daily_spending_allowances_BudgetId_Date" ON "daily_spending_allowances" ("BudgetId", "Date");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261022163648_AddBudgetsAndAdaptiveDSA', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "DevicePushTokens" (
    "Id" uuid NOT NULL CONSTRAINT "PK_DevicePushTokens" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "DeviceToken" text NOT NULL,
    "Platform" text NOT NULL,
    "DeviceId" text NULL,
    "LastActiveAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_DevicePushTokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "InAppNotifications" (
    "Id" uuid NOT NULL CONSTRAINT "PK_InAppNotifications" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Title" text NOT NULL,
    "Message" text NOT NULL,
    "Type" text NOT NULL,
    "PayloadJson" text NULL,
    "IsRead" boolean NOT NULL,
    "ReadAt" timestamp with time zone NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_InAppNotifications_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "net_worth_snapshots" (
    "Id" uuid NOT NULL CONSTRAINT "PK_net_worth_snapshots" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "SnapshotDate" timestamp with time zone NOT NULL,
    "TotalAssets" numeric(18,2) NOT NULL,
    "TotalLiabilities" numeric(18,2) NOT NULL,
    "NetWorth" numeric(18,2) NOT NULL,
    "Notes" character varying(500) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_net_worth_snapshots_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "savings_goals" (
    "Id" uuid NOT NULL CONSTRAINT "PK_savings_goals" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(1000) NULL,
    "TargetAmount" numeric(18,2) NOT NULL,
    "CurrentAmount" numeric(18,2) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "TargetDate" timestamp with time zone NOT NULL,
    "Type" integer NOT NULL,
    "Status" integer NOT NULL,
    "Priority" integer NOT NULL,
    "IconKey" character varying(50) NULL,
    "ColorHex" character varying(50) NULL,
    "CompletedAt" timestamp with time zone NULL,
    "IsAutoDepositEnabled" boolean NOT NULL DEFAULT 0,
    "AutoDepositAmount" numeric(18,2) NULL,
    "AutoDepositDayOfMonth" integer NULL,
    "AutoDepositWalletId" uuid NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

CREATE TABLE "savings_goal_contributions" (
    "Id" uuid NOT NULL CONSTRAINT "PK_savings_goal_contributions" PRIMARY KEY,
    "SavingsGoalId" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    "TransactionId" uuid NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Type" integer NOT NULL,
    "ContributionDate" timestamp with time zone NOT NULL,
    "Note" character varying(500) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "FK_savings_goal_contributions_savings_goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "savings_goals" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_savings_goal_contributions_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_savings_goal_contributions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_DevicePushTokens_UserId" ON "DevicePushTokens" ("UserId");

CREATE INDEX "IX_InAppNotifications_UserId" ON "InAppNotifications" ("UserId");

CREATE INDEX "ix_net_worth_snapshots_user_date" ON "net_worth_snapshots" ("UserId", "SnapshotDate");

CREATE INDEX "IX_savings_goal_contributions_SavingsGoalId_ContributionDate" ON "savings_goal_contributions" ("SavingsGoalId", "ContributionDate");

CREATE INDEX "IX_savings_goal_contributions_TransactionId" ON "savings_goal_contributions" ("TransactionId");

CREATE INDEX "IX_savings_goal_contributions_WalletId" ON "savings_goal_contributions" ("WalletId");

CREATE INDEX "IX_savings_goals_UserId_Status" ON "savings_goals" ("UserId", "Status");

CREATE INDEX "IX_savings_goals_UserId_TargetDate" ON "savings_goals" ("UserId", "TargetDate");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261023132227_AddNetWorthSnapshots', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
CREATE TABLE "CoolingOffTransactions" (
    "Id" uuid NOT NULL CONSTRAINT "PK_CoolingOffTransactions" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "CoolingOffUntil" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "EquivalentWorkHours" numeric(18,2) NOT NULL,
    "ProjectedDsaReduction" numeric(18,2) NOT NULL,
    "BypassReason" character varying(500) NULL,
    "ResolvedAt" timestamp with time zone NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_CoolingOffTransactions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_CoolingOffTransactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CoolingOffTransactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "FinancialNudges" (
    "Id" uuid NOT NULL CONSTRAINT "PK_FinancialNudges" PRIMARY KEY,
    "UserId" uuid NOT NULL,
    "NudgeType" integer NOT NULL,
    "Title" character varying(256) NOT NULL,
    "Message" character varying(1000) NOT NULL,
    "TriggerEvent" character varying(128) NOT NULL,
    "RationaleExplanationJson" text NULL,
    "RebalanceProposalJson" text NULL,
    "Status" integer NOT NULL,
    "IsApprovedByUser" boolean NULL,
    "ResolvedAt" timestamp with time zone NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone NULL,
    CONSTRAINT "FK_FinancialNudges_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_CoolingOffTransactions_CategoryId" ON "CoolingOffTransactions" ("CategoryId");

CREATE INDEX "IX_CoolingOffTransactions_UserId_CoolingOffUntil" ON "CoolingOffTransactions" ("UserId", "CoolingOffUntil");

CREATE INDEX "IX_CoolingOffTransactions_UserId_Status" ON "CoolingOffTransactions" ("UserId", "Status");

CREATE INDEX "IX_CoolingOffTransactions_WalletId" ON "CoolingOffTransactions" ("WalletId");

CREATE INDEX "IX_FinancialNudges_UserId_NudgeType_CreatedAt" ON "FinancialNudges" ("UserId", "NudgeType", "CreatedAt");

CREATE INDEX "IX_FinancialNudges_UserId_Status" ON "FinancialNudges" ("UserId", "Status");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261023152815_AddBehavioralNudgesAndCoolingOff', '10.0.13');

COMMIT;

BEGIN TRANSACTION;
ALTER TABLE "users" ADD "IsEmailVerified" boolean NOT NULL DEFAULT 0;

CREATE TABLE "user_otp_codes" (
    "Id" uuid NOT NULL CONSTRAINT "PK_user_otp_codes" PRIMARY KEY,
    "UserId" uuid NULL,
    "Email" character varying(255) NOT NULL,
    "PhoneNumber" character varying(32) NULL,
    "OtpCode" character varying(16) NOT NULL,
    "Purpose" integer NOT NULL,
    "ExpiredAt" timestamp without time zone NOT NULL,
    "IsUsed" boolean NOT NULL DEFAULT 0,
    "UsedAt" timestamp without time zone NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "DeletedAt" timestamp without time zone NULL,
    CONSTRAINT "FK_user_otp_codes_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_user_otp_codes_Email_Purpose_IsUsed" ON "user_otp_codes" ("Email", "Purpose", "IsUsed");

CREATE INDEX "IX_user_otp_codes_ExpiredAt" ON "user_otp_codes" ("ExpiredAt");

CREATE INDEX "IX_user_otp_codes_UserId" ON "user_otp_codes" ("UserId");

CREATE TABLE "ef_temp_WalletTransfers" (
    "Id" uuid NOT NULL CONSTRAINT "PK_WalletTransfers" PRIMARY KEY,
    "Amount" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "DestinationWalletId" uuid NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "Note" character varying(500) NULL,
    "SourceWalletId" uuid NOT NULL,
    "TransferDate" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_WalletTransfers_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_WalletTransfers_wallets_DestinationWalletId" FOREIGN KEY ("DestinationWalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_WalletTransfers_wallets_SourceWalletId" FOREIGN KEY ("SourceWalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_WalletTransfers" ("Id", "Amount", "CreatedAt", "DeletedAt", "DestinationWalletId", "IsDeleted", "Note", "SourceWalletId", "TransferDate", "UpdatedAt", "UserId")
SELECT "Id", "Amount", "CreatedAt", "DeletedAt", "DestinationWalletId", "IsDeleted", "Note", "SourceWalletId", "TransferDate", "UpdatedAt", "UserId"
FROM "WalletTransfers";

CREATE TABLE "ef_temp_wallets" (
    "Id" uuid NOT NULL CONSTRAINT "PK_wallets" PRIMARY KEY,
    "AccountNumber" character varying(50) NULL,
    "Balance" numeric(18,2) NOT NULL,
    "BankName" character varying(100) NULL,
    "ColorHex" character varying(20) NOT NULL DEFAULT '#0066FF',
    "CreatedAt" timestamp without time zone NOT NULL,
    "CreditLimit" numeric(18,2) NULL,
    "Currency" character varying(10) NOT NULL DEFAULT 'VND',
    "DeletedAt" timestamp without time zone NULL,
    "DisplayOrder" integer NOT NULL DEFAULT 0,
    "IconKey" character varying(50) NOT NULL DEFAULT 'wallet',
    "InitialBalance" numeric(18,2) NOT NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "IsExcludedFromTotal" boolean NOT NULL DEFAULT 0,
    "Name" character varying(100) NOT NULL,
    "PaymentDueDay" integer NULL,
    "StatementDay" integer NULL,
    "Status" integer NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    "Version" integer NOT NULL DEFAULT 1,
    "WalletType" integer NOT NULL,
    CONSTRAINT "FK_wallets_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_wallets" ("Id", "AccountNumber", "Balance", "BankName", "ColorHex", "CreatedAt", "CreditLimit", "Currency", "DeletedAt", "DisplayOrder", "IconKey", "InitialBalance", "IsDeleted", "IsExcludedFromTotal", "Name", "PaymentDueDay", "StatementDay", "Status", "UpdatedAt", "UserId", "Version", "WalletType")
SELECT "Id", "AccountNumber", "Balance", "BankName", "ColorHex", "CreatedAt", "CreditLimit", "Currency", "DeletedAt", "DisplayOrder", "IconKey", "InitialBalance", "IsDeleted", "IsExcludedFromTotal", "Name", "PaymentDueDay", "StatementDay", "Status", "UpdatedAt", "UserId", "Version", "WalletType"
FROM "wallets";

CREATE TABLE "ef_temp_users" (
    "Id" uuid NOT NULL CONSTRAINT "PK_users" PRIMARY KEY,
    "BaseCurrency" character varying(10) NOT NULL DEFAULT 'VND',
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "Email" character varying(255) NOT NULL,
    "FullName" character varying(100) NOT NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "IsEmailVerified" boolean NOT NULL DEFAULT 0,
    "PasswordHash" text NOT NULL,
    "Status" integer NOT NULL,
    "UpdatedAt" timestamp without time zone NULL
);

INSERT INTO "ef_temp_users" ("Id", "BaseCurrency", "CreatedAt", "DeletedAt", "Email", "FullName", "IsDeleted", "IsEmailVerified", "PasswordHash", "Status", "UpdatedAt")
SELECT "Id", "BaseCurrency", "CreatedAt", "DeletedAt", "Email", "FullName", "IsDeleted", "IsEmailVerified", "PasswordHash", "Status", "UpdatedAt"
FROM "users";

CREATE TABLE "ef_temp_transactions" (
    "Id" uuid NOT NULL CONSTRAINT "PK_transactions" PRIMARY KEY,
    "Amount" numeric(18,2) NOT NULL,
    "CategoryId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "ExcludeFromReport" boolean NOT NULL DEFAULT 0,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "IsEssential" boolean NOT NULL DEFAULT 0,
    "IsRecurring" boolean NOT NULL DEFAULT 0,
    "IsWindfall" boolean NOT NULL DEFAULT 0,
    "Note" character varying(500) NULL,
    "TransactionDate" timestamp without time zone NOT NULL,
    "TransactionType" integer NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    CONSTRAINT "FK_transactions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_transactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_transactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_transactions" ("Id", "Amount", "CategoryId", "CreatedAt", "DeletedAt", "ExcludeFromReport", "IsDeleted", "IsEssential", "IsRecurring", "IsWindfall", "Note", "TransactionDate", "TransactionType", "UpdatedAt", "UserId", "WalletId")
SELECT "Id", "Amount", "CategoryId", "CreatedAt", "DeletedAt", "ExcludeFromReport", "IsDeleted", "IsEssential", "IsRecurring", "IsWindfall", "Note", "TransactionDate", "TransactionType", "UpdatedAt", "UserId", "WalletId"
FROM "transactions";

CREATE TABLE "ef_temp_savings_goals" (
    "Id" uuid NOT NULL CONSTRAINT "PK_savings_goals" PRIMARY KEY,
    "AutoDepositAmount" decimal(18,2) NULL,
    "AutoDepositDayOfMonth" integer NULL,
    "AutoDepositWalletId" uuid NULL,
    "ColorHex" character varying(50) NULL,
    "CompletedAt" timestamp without time zone NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "CurrentAmount" decimal(18,2) NOT NULL,
    "Description" character varying(1000) NULL,
    "IconKey" character varying(50) NULL,
    "IsAutoDepositEnabled" boolean NOT NULL DEFAULT 0,
    "Name" character varying(200) NOT NULL,
    "Priority" integer NOT NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "Status" integer NOT NULL,
    "TargetAmount" decimal(18,2) NOT NULL,
    "TargetDate" timestamp without time zone NOT NULL,
    "Type" integer NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL
);

INSERT INTO "ef_temp_savings_goals" ("Id", "AutoDepositAmount", "AutoDepositDayOfMonth", "AutoDepositWalletId", "ColorHex", "CompletedAt", "CreatedAt", "CurrentAmount", "Description", "IconKey", "IsAutoDepositEnabled", "Name", "Priority", "StartDate", "Status", "TargetAmount", "TargetDate", "Type", "UpdatedAt", "UserId")
SELECT "Id", "AutoDepositAmount", "AutoDepositDayOfMonth", "AutoDepositWalletId", "ColorHex", "CompletedAt", "CreatedAt", "CurrentAmount", "Description", "IconKey", "IsAutoDepositEnabled", "Name", "Priority", "StartDate", "Status", "TargetAmount", "TargetDate", "Type", "UpdatedAt", "UserId"
FROM "savings_goals";

CREATE TABLE "ef_temp_savings_goal_contributions" (
    "Id" uuid NOT NULL CONSTRAINT "PK_savings_goal_contributions" PRIMARY KEY,
    "Amount" decimal(18,2) NOT NULL,
    "ContributionDate" timestamp without time zone NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "Note" character varying(500) NULL,
    "SavingsGoalId" uuid NOT NULL,
    "TransactionId" uuid NULL,
    "Type" integer NOT NULL,
    "WalletId" uuid NOT NULL,
    CONSTRAINT "FK_savings_goal_contributions_savings_goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "savings_goals" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_savings_goal_contributions_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_savings_goal_contributions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_savings_goal_contributions" ("Id", "Amount", "ContributionDate", "CreatedAt", "Note", "SavingsGoalId", "TransactionId", "Type", "WalletId")
SELECT "Id", "Amount", "ContributionDate", "CreatedAt", "Note", "SavingsGoalId", "TransactionId", "Type", "WalletId"
FROM "savings_goal_contributions";

CREATE TABLE "ef_temp_salary_slip_details" (
    "Id" uuid NOT NULL CONSTRAINT "PK_salary_slip_details" PRIMARY KEY,
    "Allowances" numeric(18,2) NOT NULL DEFAULT '0.0',
    "Bonus" numeric(18,2) NOT NULL DEFAULT '0.0',
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "DiscrepancyAmount" numeric(18,2) NOT NULL DEFAULT '0.0',
    "EmployerName" character varying(200) NOT NULL,
    "GrossSalary" numeric(18,2) NOT NULL,
    "HealthInsurance" numeric(18,2) NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "IsMathAudited" boolean NOT NULL DEFAULT 0,
    "NetSalary" numeric(18,2) NOT NULL,
    "OtherDeductions" numeric(18,2) NOT NULL DEFAULT '0.0',
    "PayPeriod" character varying(50) NOT NULL,
    "PersonalIncomeTax" numeric(18,2) NOT NULL,
    "RawOcrJson" text NULL,
    "SocialInsurance" numeric(18,2) NOT NULL,
    "TransactionId" uuid NOT NULL,
    "UnemploymentInsurance" numeric(18,2) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_salary_slip_details_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_salary_slip_details_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_salary_slip_details" ("Id", "Allowances", "Bonus", "CreatedAt", "DeletedAt", "DiscrepancyAmount", "EmployerName", "GrossSalary", "HealthInsurance", "IsDeleted", "IsMathAudited", "NetSalary", "OtherDeductions", "PayPeriod", "PersonalIncomeTax", "RawOcrJson", "SocialInsurance", "TransactionId", "UnemploymentInsurance", "UpdatedAt", "UserId")
SELECT "Id", "Allowances", "Bonus", "CreatedAt", "DeletedAt", "DiscrepancyAmount", "EmployerName", "GrossSalary", "HealthInsurance", "IsDeleted", "IsMathAudited", "NetSalary", "OtherDeductions", "PayPeriod", "PersonalIncomeTax", "RawOcrJson", "SocialInsurance", "TransactionId", "UnemploymentInsurance", "UpdatedAt", "UserId"
FROM "salary_slip_details";

CREATE TABLE "ef_temp_refresh_tokens" (
    "Id" uuid NOT NULL CONSTRAINT "PK_refresh_tokens" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "ExpiresAt" timestamp without time zone NOT NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "ReplacedByToken" character varying(255) NULL,
    "RevokedAt" timestamp without time zone NULL,
    "Token" character varying(255) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_refresh_tokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_refresh_tokens" ("Id", "CreatedAt", "DeletedAt", "ExpiresAt", "IsDeleted", "ReplacedByToken", "RevokedAt", "Token", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "ExpiresAt", "IsDeleted", "ReplacedByToken", "RevokedAt", "Token", "UpdatedAt", "UserId"
FROM "refresh_tokens";

CREATE TABLE "ef_temp_recurring_incomes" (
    "Id" uuid NOT NULL CONSTRAINT "PK_recurring_incomes" PRIMARY KEY,
    "AutoRecord" boolean NOT NULL DEFAULT 1,
    "CategoryId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DayOfMonth" integer NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "ExpectedAmount" numeric(18,2) NOT NULL,
    "Frequency" integer NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT 1,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "LastGeneratedDate" timestamp without time zone NULL,
    "Name" character varying(150) NOT NULL,
    "NextDueDate" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    CONSTRAINT "FK_recurring_incomes_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recurring_incomes_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_recurring_incomes_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_recurring_incomes" ("Id", "AutoRecord", "CategoryId", "CreatedAt", "DayOfMonth", "DeletedAt", "ExpectedAmount", "Frequency", "IsActive", "IsDeleted", "LastGeneratedDate", "Name", "NextDueDate", "UpdatedAt", "UserId", "WalletId")
SELECT "Id", "AutoRecord", "CategoryId", "CreatedAt", "DayOfMonth", "DeletedAt", "ExpectedAmount", "Frequency", "IsActive", "IsDeleted", "LastGeneratedDate", "Name", "NextDueDate", "UpdatedAt", "UserId", "WalletId"
FROM "recurring_incomes";

CREATE TABLE "ef_temp_receipt_items" (
    "Id" uuid NOT NULL CONSTRAINT "PK_receipt_items" PRIMARY KEY,
    "CategoryId" uuid NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "ItemName" character varying(200) NOT NULL,
    "Quantity" numeric(18,3) NOT NULL,
    "TotalPrice" numeric(18,2) NOT NULL,
    "TransactionId" uuid NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    CONSTRAINT "FK_receipt_items_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_receipt_items_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_receipt_items" ("Id", "CategoryId", "CreatedAt", "DeletedAt", "IsDeleted", "ItemName", "Quantity", "TotalPrice", "TransactionId", "UnitPrice", "UpdatedAt")
SELECT "Id", "CategoryId", "CreatedAt", "DeletedAt", "IsDeleted", "ItemName", "Quantity", "TotalPrice", "TransactionId", "UnitPrice", "UpdatedAt"
FROM "receipt_items";

CREATE TABLE "ef_temp_net_worth_snapshots" (
    "Id" uuid NOT NULL CONSTRAINT "PK_net_worth_snapshots" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "NetWorth" decimal(18,2) NOT NULL,
    "Notes" character varying(500) NULL,
    "SnapshotDate" timestamp without time zone NOT NULL,
    "TotalAssets" decimal(18,2) NOT NULL,
    "TotalLiabilities" decimal(18,2) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_net_worth_snapshots_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_net_worth_snapshots" ("Id", "CreatedAt", "DeletedAt", "IsDeleted", "NetWorth", "Notes", "SnapshotDate", "TotalAssets", "TotalLiabilities", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsDeleted", "NetWorth", "Notes", "SnapshotDate", "TotalAssets", "TotalLiabilities", "UpdatedAt", "UserId"
FROM "net_worth_snapshots";

CREATE TABLE "ef_temp_InAppNotifications" (
    "Id" uuid NOT NULL CONSTRAINT "PK_InAppNotifications" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "IsDeleted" boolean NOT NULL,
    "IsRead" boolean NOT NULL,
    "Message" text NOT NULL,
    "PayloadJson" text NULL,
    "ReadAt" timestamp without time zone NULL,
    "Title" text NOT NULL,
    "Type" text NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_InAppNotifications_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_InAppNotifications" ("Id", "CreatedAt", "DeletedAt", "IsDeleted", "IsRead", "Message", "PayloadJson", "ReadAt", "Title", "Type", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsDeleted", "IsRead", "Message", "PayloadJson", "ReadAt", "Title", "Type", "UpdatedAt", "UserId"
FROM "InAppNotifications";

CREATE TABLE "ef_temp_FinancialNudges" (
    "Id" uuid NOT NULL CONSTRAINT "PK_FinancialNudges" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "IsApprovedByUser" boolean NULL,
    "IsDeleted" boolean NOT NULL,
    "Message" character varying(1000) NOT NULL,
    "NudgeType" integer NOT NULL,
    "RationaleExplanationJson" text NULL,
    "RebalanceProposalJson" text NULL,
    "ResolvedAt" timestamp without time zone NULL,
    "Status" integer NOT NULL,
    "Title" character varying(256) NOT NULL,
    "TriggerEvent" character varying(128) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_FinancialNudges_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_FinancialNudges" ("Id", "CreatedAt", "DeletedAt", "IsApprovedByUser", "IsDeleted", "Message", "NudgeType", "RationaleExplanationJson", "RebalanceProposalJson", "ResolvedAt", "Status", "Title", "TriggerEvent", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsApprovedByUser", "IsDeleted", "Message", "NudgeType", "RationaleExplanationJson", "RebalanceProposalJson", "ResolvedAt", "Status", "Title", "TriggerEvent", "UpdatedAt", "UserId"
FROM "FinancialNudges";

CREATE TABLE "ef_temp_DevicePushTokens" (
    "Id" uuid NOT NULL CONSTRAINT "PK_DevicePushTokens" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "DeviceId" text NULL,
    "DeviceToken" text NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "LastActiveAt" timestamp without time zone NOT NULL,
    "Platform" text NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_DevicePushTokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_DevicePushTokens" ("Id", "CreatedAt", "DeletedAt", "DeviceId", "DeviceToken", "IsDeleted", "LastActiveAt", "Platform", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "DeviceId", "DeviceToken", "IsDeleted", "LastActiveAt", "Platform", "UpdatedAt", "UserId"
FROM "DevicePushTokens";

CREATE TABLE "ef_temp_daily_spending_allowances" (
    "Id" uuid NOT NULL CONSTRAINT "PK_daily_spending_allowances" PRIMARY KEY,
    "ActualSpentToday" decimal(18,2) NOT NULL,
    "BudgetId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DailyAllowanceAmount" decimal(18,2) NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "OutOfMoneyForecastDate" timestamp without time zone NULL,
    "RemainingAllowanceToday" decimal(18,2) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "VelocityIndex" decimal(8,4) NOT NULL DEFAULT '1.0',
    CONSTRAINT "FK_daily_spending_allowances_budgets_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "budgets" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_daily_spending_allowances" ("Id", "ActualSpentToday", "BudgetId", "CreatedAt", "DailyAllowanceAmount", "Date", "OutOfMoneyForecastDate", "RemainingAllowanceToday", "UpdatedAt", "VelocityIndex")
SELECT "Id", "ActualSpentToday", "BudgetId", "CreatedAt", "DailyAllowanceAmount", "Date", "OutOfMoneyForecastDate", "RemainingAllowanceToday", "UpdatedAt", "VelocityIndex"
FROM "daily_spending_allowances";

CREATE TABLE "ef_temp_CoolingOffTransactions" (
    "Id" uuid NOT NULL CONSTRAINT "PK_CoolingOffTransactions" PRIMARY KEY,
    "Amount" numeric(18,2) NOT NULL,
    "BypassReason" character varying(500) NULL,
    "CategoryId" uuid NOT NULL,
    "CoolingOffUntil" timestamp without time zone NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "Description" character varying(500) NOT NULL,
    "EquivalentWorkHours" numeric(18,2) NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "ProjectedDsaReduction" numeric(18,2) NOT NULL,
    "ResolvedAt" timestamp without time zone NULL,
    "Status" integer NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    "WalletId" uuid NOT NULL,
    CONSTRAINT "FK_CoolingOffTransactions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_CoolingOffTransactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CoolingOffTransactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_CoolingOffTransactions" ("Id", "Amount", "BypassReason", "CategoryId", "CoolingOffUntil", "CreatedAt", "DeletedAt", "Description", "EquivalentWorkHours", "IsDeleted", "ProjectedDsaReduction", "ResolvedAt", "Status", "UpdatedAt", "UserId", "WalletId")
SELECT "Id", "Amount", "BypassReason", "CategoryId", "CoolingOffUntil", "CreatedAt", "DeletedAt", "Description", "EquivalentWorkHours", "IsDeleted", "ProjectedDsaReduction", "ResolvedAt", "Status", "UpdatedAt", "UserId", "WalletId"
FROM "CoolingOffTransactions";

CREATE TABLE "ef_temp_categories" (
    "Id" uuid NOT NULL CONSTRAINT "PK_categories" PRIMARY KEY,
    "BudgetRuleMapping" integer NULL,
    "ColorHex" character varying(20) NOT NULL DEFAULT '#10B981',
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "DisplayOrder" integer NOT NULL DEFAULT 0,
    "IconKey" character varying(50) NOT NULL DEFAULT 'folder',
    "IncomeGroup" integer NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "IsSystem" boolean NOT NULL DEFAULT 0,
    "Name" character varying(100) NOT NULL,
    "ParentId" uuid NULL,
    "TransactionType" integer NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NULL,
    CONSTRAINT "FK_categories_categories_ParentId" FOREIGN KEY ("ParentId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_categories_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_categories" ("Id", "BudgetRuleMapping", "ColorHex", "CreatedAt", "DeletedAt", "DisplayOrder", "IconKey", "IncomeGroup", "IsDeleted", "IsSystem", "Name", "ParentId", "TransactionType", "UpdatedAt", "UserId")
SELECT "Id", "BudgetRuleMapping", "ColorHex", "CreatedAt", "DeletedAt", "DisplayOrder", "IconKey", "IncomeGroup", "IsDeleted", "IsSystem", "Name", "ParentId", "TransactionType", "UpdatedAt", "UserId"
FROM "categories";

CREATE TABLE "ef_temp_budgets" (
    "Id" uuid NOT NULL CONSTRAINT "PK_budgets" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "EndDate" timestamp without time zone NOT NULL,
    "Method" integer NOT NULL,
    "Name" character varying(200) NOT NULL,
    "PeriodType" integer NOT NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "Status" integer NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL
);

INSERT INTO "ef_temp_budgets" ("Id", "CreatedAt", "EndDate", "Method", "Name", "PeriodType", "StartDate", "Status", "TotalAmount", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "EndDate", "Method", "Name", "PeriodType", "StartDate", "Status", "TotalAmount", "UpdatedAt", "UserId"
FROM "budgets";

CREATE TABLE "ef_temp_balance_adjustments" (
    "Id" uuid NOT NULL CONSTRAINT "PK_balance_adjustments" PRIMARY KEY,
    "AdjustmentDate" timestamp without time zone NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "DifferenceAmount" numeric(18,2) NOT NULL,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "NewBalance" numeric(18,2) NOT NULL,
    "OldBalance" numeric(18,2) NOT NULL,
    "ProofImageUrl" character varying(500) NULL,
    "Reason" character varying(255) NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "WalletId" uuid NOT NULL,
    CONSTRAINT "FK_balance_adjustments_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_balance_adjustments" ("Id", "AdjustmentDate", "CreatedAt", "DeletedAt", "DifferenceAmount", "IsDeleted", "NewBalance", "OldBalance", "ProofImageUrl", "Reason", "UpdatedAt", "WalletId")
SELECT "Id", "AdjustmentDate", "CreatedAt", "DeletedAt", "DifferenceAmount", "IsDeleted", "NewBalance", "OldBalance", "ProofImageUrl", "Reason", "UpdatedAt", "WalletId"
FROM "balance_adjustments";

CREATE TABLE "ef_temp_auto_categorization_rules" (
    "Id" uuid NOT NULL CONSTRAINT "PK_auto_categorization_rules" PRIMARY KEY,
    "CreatedAt" timestamp without time zone NOT NULL,
    "DeletedAt" timestamp without time zone NULL,
    "IsCaseSensitive" boolean NOT NULL DEFAULT 0,
    "IsDeleted" boolean NOT NULL DEFAULT 0,
    "IsRegex" boolean NOT NULL DEFAULT 0,
    "KeywordPattern" character varying(100) NOT NULL,
    "PriorityOrder" integer NOT NULL DEFAULT 0,
    "TargetCategoryId" uuid NOT NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "FK_auto_categorization_rules_categories_TargetCategoryId" FOREIGN KEY ("TargetCategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_auto_categorization_rules_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_auto_categorization_rules" ("Id", "CreatedAt", "DeletedAt", "IsCaseSensitive", "IsDeleted", "IsRegex", "KeywordPattern", "PriorityOrder", "TargetCategoryId", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsCaseSensitive", "IsDeleted", "IsRegex", "KeywordPattern", "PriorityOrder", "TargetCategoryId", "UpdatedAt", "UserId"
FROM "auto_categorization_rules";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "WalletTransfers";

ALTER TABLE "ef_temp_WalletTransfers" RENAME TO "WalletTransfers";

DROP TABLE "wallets";

ALTER TABLE "ef_temp_wallets" RENAME TO "wallets";

DROP TABLE "users";

ALTER TABLE "ef_temp_users" RENAME TO "users";

DROP TABLE "transactions";

ALTER TABLE "ef_temp_transactions" RENAME TO "transactions";

DROP TABLE "savings_goals";

ALTER TABLE "ef_temp_savings_goals" RENAME TO "savings_goals";

DROP TABLE "savings_goal_contributions";

ALTER TABLE "ef_temp_savings_goal_contributions" RENAME TO "savings_goal_contributions";

DROP TABLE "salary_slip_details";

ALTER TABLE "ef_temp_salary_slip_details" RENAME TO "salary_slip_details";

DROP TABLE "refresh_tokens";

ALTER TABLE "ef_temp_refresh_tokens" RENAME TO "refresh_tokens";

DROP TABLE "recurring_incomes";

ALTER TABLE "ef_temp_recurring_incomes" RENAME TO "recurring_incomes";

DROP TABLE "receipt_items";

ALTER TABLE "ef_temp_receipt_items" RENAME TO "receipt_items";

DROP TABLE "net_worth_snapshots";

ALTER TABLE "ef_temp_net_worth_snapshots" RENAME TO "net_worth_snapshots";

DROP TABLE "InAppNotifications";

ALTER TABLE "ef_temp_InAppNotifications" RENAME TO "InAppNotifications";

DROP TABLE "FinancialNudges";

ALTER TABLE "ef_temp_FinancialNudges" RENAME TO "FinancialNudges";

DROP TABLE "DevicePushTokens";

ALTER TABLE "ef_temp_DevicePushTokens" RENAME TO "DevicePushTokens";

DROP TABLE "daily_spending_allowances";

ALTER TABLE "ef_temp_daily_spending_allowances" RENAME TO "daily_spending_allowances";

DROP TABLE "CoolingOffTransactions";

ALTER TABLE "ef_temp_CoolingOffTransactions" RENAME TO "CoolingOffTransactions";

DROP TABLE "categories";

ALTER TABLE "ef_temp_categories" RENAME TO "categories";

DROP TABLE "budgets";

ALTER TABLE "ef_temp_budgets" RENAME TO "budgets";

DROP TABLE "balance_adjustments";

ALTER TABLE "ef_temp_balance_adjustments" RENAME TO "balance_adjustments";

DROP TABLE "auto_categorization_rules";

ALTER TABLE "ef_temp_auto_categorization_rules" RENAME TO "auto_categorization_rules";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;
CREATE INDEX "IX_WalletTransfers_DestinationWalletId" ON "WalletTransfers" ("DestinationWalletId");

CREATE INDEX "IX_WalletTransfers_SourceWalletId" ON "WalletTransfers" ("SourceWalletId");

CREATE INDEX "IX_WalletTransfers_UserId_TransferDate" ON "WalletTransfers" ("UserId", "TransferDate");

CREATE INDEX "IX_wallets_UserId_Status" ON "wallets" ("UserId", "Status");

CREATE UNIQUE INDEX "IX_users_Email" ON "users" ("Email");

CREATE INDEX "IX_transactions_CategoryId_IsDeleted" ON "transactions" ("CategoryId", "IsDeleted");

CREATE INDEX "IX_transactions_UserId_TransactionDate_IsDeleted" ON "transactions" ("UserId", "TransactionDate", "IsDeleted");

CREATE INDEX "IX_transactions_UserId_TransactionType_TransactionDate" ON "transactions" ("UserId", "TransactionType", "TransactionDate");

CREATE INDEX "IX_transactions_WalletId_IsDeleted" ON "transactions" ("WalletId", "IsDeleted");

CREATE INDEX "IX_savings_goals_UserId_Status" ON "savings_goals" ("UserId", "Status");

CREATE INDEX "IX_savings_goals_UserId_TargetDate" ON "savings_goals" ("UserId", "TargetDate");

CREATE INDEX "IX_savings_goal_contributions_SavingsGoalId_ContributionDate" ON "savings_goal_contributions" ("SavingsGoalId", "ContributionDate");

CREATE INDEX "IX_savings_goal_contributions_TransactionId" ON "savings_goal_contributions" ("TransactionId");

CREATE INDEX "IX_savings_goal_contributions_WalletId" ON "savings_goal_contributions" ("WalletId");

CREATE UNIQUE INDEX "IX_salary_slip_details_TransactionId" ON "salary_slip_details" ("TransactionId");

CREATE INDEX "IX_salary_slip_details_UserId_PayPeriod" ON "salary_slip_details" ("UserId", "PayPeriod");

CREATE INDEX "IX_refresh_tokens_Token" ON "refresh_tokens" ("Token");

CREATE INDEX "IX_refresh_tokens_UserId" ON "refresh_tokens" ("UserId");

CREATE INDEX "IX_recurring_incomes_CategoryId" ON "recurring_incomes" ("CategoryId");

CREATE INDEX "IX_recurring_incomes_IsActive_AutoRecord_NextDueDate" ON "recurring_incomes" ("IsActive", "AutoRecord", "NextDueDate");

CREATE INDEX "IX_recurring_incomes_UserId_IsDeleted" ON "recurring_incomes" ("UserId", "IsDeleted");

CREATE INDEX "IX_recurring_incomes_WalletId" ON "recurring_incomes" ("WalletId");

CREATE INDEX "IX_receipt_items_CategoryId" ON "receipt_items" ("CategoryId");

CREATE INDEX "IX_receipt_items_TransactionId" ON "receipt_items" ("TransactionId");

CREATE INDEX "ix_net_worth_snapshots_user_date" ON "net_worth_snapshots" ("UserId", "SnapshotDate");

CREATE INDEX "IX_InAppNotifications_UserId" ON "InAppNotifications" ("UserId");

CREATE INDEX "IX_FinancialNudges_UserId_NudgeType_CreatedAt" ON "FinancialNudges" ("UserId", "NudgeType", "CreatedAt");

CREATE INDEX "IX_FinancialNudges_UserId_Status" ON "FinancialNudges" ("UserId", "Status");

CREATE INDEX "IX_DevicePushTokens_UserId" ON "DevicePushTokens" ("UserId");

CREATE UNIQUE INDEX "IX_daily_spending_allowances_BudgetId_Date" ON "daily_spending_allowances" ("BudgetId", "Date");

CREATE INDEX "IX_CoolingOffTransactions_CategoryId" ON "CoolingOffTransactions" ("CategoryId");

CREATE INDEX "IX_CoolingOffTransactions_UserId_CoolingOffUntil" ON "CoolingOffTransactions" ("UserId", "CoolingOffUntil");

CREATE INDEX "IX_CoolingOffTransactions_UserId_Status" ON "CoolingOffTransactions" ("UserId", "Status");

CREATE INDEX "IX_CoolingOffTransactions_WalletId" ON "CoolingOffTransactions" ("WalletId");

CREATE INDEX "IX_categories_IsSystem" ON "categories" ("IsSystem");

CREATE INDEX "IX_categories_ParentId" ON "categories" ("ParentId");

CREATE INDEX "IX_categories_UserId_TransactionType_IsDeleted" ON "categories" ("UserId", "TransactionType", "IsDeleted");

CREATE INDEX "IX_budgets_UserId_StartDate_EndDate" ON "budgets" ("UserId", "StartDate", "EndDate");

CREATE INDEX "IX_budgets_UserId_Status" ON "budgets" ("UserId", "Status");

CREATE INDEX "IX_balance_adjustments_WalletId_AdjustmentDate" ON "balance_adjustments" ("WalletId", "AdjustmentDate");

CREATE INDEX "IX_auto_categorization_rules_TargetCategoryId" ON "auto_categorization_rules" ("TargetCategoryId");

CREATE INDEX "IX_auto_categorization_rules_UserId_KeywordPattern" ON "auto_categorization_rules" ("UserId", "KeywordPattern");

CREATE INDEX "IX_auto_categorization_rules_UserId_PriorityOrder" ON "auto_categorization_rules" ("UserId", "PriorityOrder");

COMMIT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261026162952_AddUserOtpCodes', '10.0.13');

BEGIN TRANSACTION;
CREATE TABLE "debts" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_debts" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "DebtType" INTEGER NOT NULL,
    "CreditorName" TEXT NOT NULL,
    "PrincipalAmount" decimal(18,2) NOT NULL,
    "CurrentBalance" decimal(18,2) NOT NULL,
    "InterestRateAnnual" decimal(5,2) NOT NULL,
    "MinimumPayment" decimal(18,2) NOT NULL,
    "DueDateMonthly" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "ExpectedPayoffDate" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    CONSTRAINT "FK_debts_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "subscriptions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_subscriptions" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Amount" decimal(18,2) NOT NULL,
    "BillingCycle" INTEGER NOT NULL,
    "NextBillingDate" TEXT NOT NULL,
    "WalletId" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "TrialEndDate" TEXT NULL,
    "AutoRenew" INTEGER NOT NULL DEFAULT 1,
    "RemindDaysBefore" INTEGER NOT NULL DEFAULT 3,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    CONSTRAINT "FK_subscriptions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_subscriptions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_subscriptions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE SET NULL
);

CREATE TABLE "debt_transactions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_debt_transactions" PRIMARY KEY,
    "DebtId" TEXT NOT NULL,
    "WalletId" TEXT NOT NULL,
    "Amount" decimal(18,2) NOT NULL,
    "PrincipalPaid" decimal(18,2) NOT NULL,
    "InterestPaid" decimal(18,2) NOT NULL,
    "PaymentDate" TEXT NOT NULL,
    "Note" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    CONSTRAINT "FK_debt_transactions_debts_DebtId" FOREIGN KEY ("DebtId") REFERENCES "debts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_debt_transactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_debt_transactions_DebtId" ON "debt_transactions" ("DebtId");

CREATE INDEX "IX_debt_transactions_PaymentDate" ON "debt_transactions" ("PaymentDate");

CREATE INDEX "IX_debt_transactions_WalletId" ON "debt_transactions" ("WalletId");

CREATE INDEX "IX_debts_UserId" ON "debts" ("UserId");

CREATE INDEX "IX_debts_UserId_Status" ON "debts" ("UserId", "Status");

CREATE INDEX "IX_subscriptions_CategoryId" ON "subscriptions" ("CategoryId");

CREATE INDEX "IX_subscriptions_NextBillingDate" ON "subscriptions" ("NextBillingDate");

CREATE INDEX "IX_subscriptions_UserId" ON "subscriptions" ("UserId");

CREATE INDEX "IX_subscriptions_UserId_Status" ON "subscriptions" ("UserId", "Status");

CREATE INDEX "IX_subscriptions_WalletId" ON "subscriptions" ("WalletId");

CREATE TABLE "ef_temp_WalletTransfers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_WalletTransfers" PRIMARY KEY,
    "Amount" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "DestinationWalletId" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "Note" TEXT NULL,
    "SourceWalletId" TEXT NOT NULL,
    "TransferDate" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_WalletTransfers_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_WalletTransfers_wallets_DestinationWalletId" FOREIGN KEY ("DestinationWalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_WalletTransfers_wallets_SourceWalletId" FOREIGN KEY ("SourceWalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_WalletTransfers" ("Id", "Amount", "CreatedAt", "DeletedAt", "DestinationWalletId", "IsDeleted", "Note", "SourceWalletId", "TransferDate", "UpdatedAt", "UserId")
SELECT "Id", "Amount", "CreatedAt", "DeletedAt", "DestinationWalletId", "IsDeleted", "Note", "SourceWalletId", "TransferDate", "UpdatedAt", "UserId"
FROM "WalletTransfers";

CREATE TABLE "ef_temp_wallets" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_wallets" PRIMARY KEY,
    "AccountNumber" TEXT NULL,
    "Balance" TEXT NOT NULL,
    "BankName" TEXT NULL,
    "ColorHex" TEXT NOT NULL DEFAULT '#0066FF',
    "CreatedAt" TEXT NOT NULL,
    "CreditLimit" TEXT NULL,
    "Currency" TEXT NOT NULL DEFAULT 'VND',
    "DeletedAt" TEXT NULL,
    "DisplayOrder" INTEGER NOT NULL DEFAULT 0,
    "IconKey" TEXT NOT NULL DEFAULT 'wallet',
    "InitialBalance" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "IsExcludedFromTotal" INTEGER NOT NULL DEFAULT 0,
    "Name" TEXT NOT NULL,
    "PaymentDueDay" INTEGER NULL,
    "StatementDay" INTEGER NULL,
    "Status" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    "Version" INTEGER NOT NULL DEFAULT 1,
    "WalletType" INTEGER NOT NULL,
    CONSTRAINT "FK_wallets_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_wallets" ("Id", "AccountNumber", "Balance", "BankName", "ColorHex", "CreatedAt", "CreditLimit", "Currency", "DeletedAt", "DisplayOrder", "IconKey", "InitialBalance", "IsDeleted", "IsExcludedFromTotal", "Name", "PaymentDueDay", "StatementDay", "Status", "UpdatedAt", "UserId", "Version", "WalletType")
SELECT "Id", "AccountNumber", "Balance", "BankName", "ColorHex", "CreatedAt", "CreditLimit", "Currency", "DeletedAt", "DisplayOrder", "IconKey", "InitialBalance", "IsDeleted", "IsExcludedFromTotal", "Name", "PaymentDueDay", "StatementDay", "Status", "UpdatedAt", "UserId", "Version", "WalletType"
FROM "wallets";

CREATE TABLE "ef_temp_users" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_users" PRIMARY KEY,
    "BaseCurrency" TEXT NOT NULL DEFAULT 'VND',
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "Email" TEXT NOT NULL,
    "FullName" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "IsEmailVerified" INTEGER NOT NULL DEFAULT 0,
    "PasswordHash" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL
);

INSERT INTO "ef_temp_users" ("Id", "BaseCurrency", "CreatedAt", "DeletedAt", "Email", "FullName", "IsDeleted", "IsEmailVerified", "PasswordHash", "Status", "UpdatedAt")
SELECT "Id", "BaseCurrency", "CreatedAt", "DeletedAt", "Email", "FullName", "IsDeleted", "IsEmailVerified", "PasswordHash", "Status", "UpdatedAt"
FROM "users";

CREATE TABLE "ef_temp_user_otp_codes" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_user_otp_codes" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "Email" TEXT NOT NULL,
    "ExpiredAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "IsUsed" INTEGER NOT NULL DEFAULT 0,
    "OtpCode" TEXT NOT NULL,
    "PhoneNumber" TEXT NULL,
    "Purpose" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UsedAt" TEXT NULL,
    "UserId" TEXT NULL,
    CONSTRAINT "FK_user_otp_codes_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_user_otp_codes" ("Id", "CreatedAt", "DeletedAt", "Email", "ExpiredAt", "IsDeleted", "IsUsed", "OtpCode", "PhoneNumber", "Purpose", "UpdatedAt", "UsedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "Email", "ExpiredAt", "IsDeleted", "IsUsed", "OtpCode", "PhoneNumber", "Purpose", "UpdatedAt", "UsedAt", "UserId"
FROM "user_otp_codes";

CREATE TABLE "ef_temp_transactions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_transactions" PRIMARY KEY,
    "Amount" TEXT NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "ExcludeFromReport" INTEGER NOT NULL DEFAULT 0,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "IsEssential" INTEGER NOT NULL DEFAULT 0,
    "IsRecurring" INTEGER NOT NULL DEFAULT 0,
    "IsWindfall" INTEGER NOT NULL DEFAULT 0,
    "Note" TEXT NULL,
    "TransactionDate" TEXT NOT NULL,
    "TransactionType" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    "WalletId" TEXT NOT NULL,
    CONSTRAINT "FK_transactions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_transactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_transactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_transactions" ("Id", "Amount", "CategoryId", "CreatedAt", "DeletedAt", "ExcludeFromReport", "IsDeleted", "IsEssential", "IsRecurring", "IsWindfall", "Note", "TransactionDate", "TransactionType", "UpdatedAt", "UserId", "WalletId")
SELECT "Id", "Amount", "CategoryId", "CreatedAt", "DeletedAt", "ExcludeFromReport", "IsDeleted", "IsEssential", "IsRecurring", "IsWindfall", "Note", "TransactionDate", "TransactionType", "UpdatedAt", "UserId", "WalletId"
FROM "transactions";

CREATE TABLE "ef_temp_savings_goals" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_savings_goals" PRIMARY KEY,
    "AutoDepositAmount" decimal(18,2) NULL,
    "AutoDepositDayOfMonth" INTEGER NULL,
    "AutoDepositWalletId" TEXT NULL,
    "ColorHex" TEXT NULL,
    "CompletedAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "CurrentAmount" decimal(18,2) NOT NULL,
    "Description" TEXT NULL,
    "IconKey" TEXT NULL,
    "IsAutoDepositEnabled" INTEGER NOT NULL DEFAULT 0,
    "Name" TEXT NOT NULL,
    "Priority" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "TargetAmount" decimal(18,2) NOT NULL,
    "TargetDate" TEXT NOT NULL,
    "Type" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL
);

INSERT INTO "ef_temp_savings_goals" ("Id", "AutoDepositAmount", "AutoDepositDayOfMonth", "AutoDepositWalletId", "ColorHex", "CompletedAt", "CreatedAt", "CurrentAmount", "Description", "IconKey", "IsAutoDepositEnabled", "Name", "Priority", "StartDate", "Status", "TargetAmount", "TargetDate", "Type", "UpdatedAt", "UserId")
SELECT "Id", "AutoDepositAmount", "AutoDepositDayOfMonth", "AutoDepositWalletId", "ColorHex", "CompletedAt", "CreatedAt", "CurrentAmount", "Description", "IconKey", "IsAutoDepositEnabled", "Name", "Priority", "StartDate", "Status", "TargetAmount", "TargetDate", "Type", "UpdatedAt", "UserId"
FROM "savings_goals";

CREATE TABLE "ef_temp_savings_goal_contributions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_savings_goal_contributions" PRIMARY KEY,
    "Amount" decimal(18,2) NOT NULL,
    "ContributionDate" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "Note" TEXT NULL,
    "SavingsGoalId" TEXT NOT NULL,
    "TransactionId" TEXT NULL,
    "Type" INTEGER NOT NULL,
    "WalletId" TEXT NOT NULL,
    CONSTRAINT "FK_savings_goal_contributions_savings_goals_SavingsGoalId" FOREIGN KEY ("SavingsGoalId") REFERENCES "savings_goals" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_savings_goal_contributions_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_savings_goal_contributions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_savings_goal_contributions" ("Id", "Amount", "ContributionDate", "CreatedAt", "Note", "SavingsGoalId", "TransactionId", "Type", "WalletId")
SELECT "Id", "Amount", "ContributionDate", "CreatedAt", "Note", "SavingsGoalId", "TransactionId", "Type", "WalletId"
FROM "savings_goal_contributions";

CREATE TABLE "ef_temp_salary_slip_details" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_salary_slip_details" PRIMARY KEY,
    "Allowances" TEXT NOT NULL DEFAULT '0.0',
    "Bonus" TEXT NOT NULL DEFAULT '0.0',
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "DiscrepancyAmount" TEXT NOT NULL DEFAULT '0.0',
    "EmployerName" TEXT NOT NULL,
    "GrossSalary" TEXT NOT NULL,
    "HealthInsurance" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsMathAudited" INTEGER NOT NULL DEFAULT 0,
    "NetSalary" TEXT NOT NULL,
    "OtherDeductions" TEXT NOT NULL DEFAULT '0.0',
    "PayPeriod" TEXT NOT NULL,
    "PersonalIncomeTax" TEXT NOT NULL,
    "RawOcrJson" text NULL,
    "SocialInsurance" TEXT NOT NULL,
    "TransactionId" TEXT NOT NULL,
    "UnemploymentInsurance" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_salary_slip_details_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_salary_slip_details_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_salary_slip_details" ("Id", "Allowances", "Bonus", "CreatedAt", "DeletedAt", "DiscrepancyAmount", "EmployerName", "GrossSalary", "HealthInsurance", "IsDeleted", "IsMathAudited", "NetSalary", "OtherDeductions", "PayPeriod", "PersonalIncomeTax", "RawOcrJson", "SocialInsurance", "TransactionId", "UnemploymentInsurance", "UpdatedAt", "UserId")
SELECT "Id", "Allowances", "Bonus", "CreatedAt", "DeletedAt", "DiscrepancyAmount", "EmployerName", "GrossSalary", "HealthInsurance", "IsDeleted", "IsMathAudited", "NetSalary", "OtherDeductions", "PayPeriod", "PersonalIncomeTax", "RawOcrJson", "SocialInsurance", "TransactionId", "UnemploymentInsurance", "UpdatedAt", "UserId"
FROM "salary_slip_details";

CREATE TABLE "ef_temp_refresh_tokens" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_refresh_tokens" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "ExpiresAt" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "ReplacedByToken" TEXT NULL,
    "RevokedAt" TEXT NULL,
    "Token" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_refresh_tokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_refresh_tokens" ("Id", "CreatedAt", "DeletedAt", "ExpiresAt", "IsDeleted", "ReplacedByToken", "RevokedAt", "Token", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "ExpiresAt", "IsDeleted", "ReplacedByToken", "RevokedAt", "Token", "UpdatedAt", "UserId"
FROM "refresh_tokens";

CREATE TABLE "ef_temp_recurring_incomes" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_recurring_incomes" PRIMARY KEY,
    "AutoRecord" INTEGER NOT NULL DEFAULT 1,
    "CategoryId" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DayOfMonth" INTEGER NOT NULL,
    "DeletedAt" TEXT NULL,
    "ExpectedAmount" TEXT NOT NULL,
    "Frequency" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "LastGeneratedDate" TEXT NULL,
    "Name" TEXT NOT NULL,
    "NextDueDate" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    "WalletId" TEXT NOT NULL,
    CONSTRAINT "FK_recurring_incomes_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_recurring_incomes_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_recurring_incomes_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_recurring_incomes" ("Id", "AutoRecord", "CategoryId", "CreatedAt", "DayOfMonth", "DeletedAt", "ExpectedAmount", "Frequency", "IsActive", "IsDeleted", "LastGeneratedDate", "Name", "NextDueDate", "UpdatedAt", "UserId", "WalletId")
SELECT "Id", "AutoRecord", "CategoryId", "CreatedAt", "DayOfMonth", "DeletedAt", "ExpectedAmount", "Frequency", "IsActive", "IsDeleted", "LastGeneratedDate", "Name", "NextDueDate", "UpdatedAt", "UserId", "WalletId"
FROM "recurring_incomes";

CREATE TABLE "ef_temp_receipt_items" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_receipt_items" PRIMARY KEY,
    "CategoryId" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "ItemName" TEXT NOT NULL,
    "Quantity" TEXT NOT NULL,
    "TotalPrice" TEXT NOT NULL,
    "TransactionId" TEXT NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    CONSTRAINT "FK_receipt_items_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_receipt_items_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "transactions" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_receipt_items" ("Id", "CategoryId", "CreatedAt", "DeletedAt", "IsDeleted", "ItemName", "Quantity", "TotalPrice", "TransactionId", "UnitPrice", "UpdatedAt")
SELECT "Id", "CategoryId", "CreatedAt", "DeletedAt", "IsDeleted", "ItemName", "Quantity", "TotalPrice", "TransactionId", "UnitPrice", "UpdatedAt"
FROM "receipt_items";

CREATE TABLE "ef_temp_net_worth_snapshots" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_net_worth_snapshots" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "NetWorth" decimal(18,2) NOT NULL,
    "Notes" TEXT NULL,
    "SnapshotDate" TEXT NOT NULL,
    "TotalAssets" decimal(18,2) NOT NULL,
    "TotalLiabilities" decimal(18,2) NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_net_worth_snapshots_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_net_worth_snapshots" ("Id", "CreatedAt", "DeletedAt", "IsDeleted", "NetWorth", "Notes", "SnapshotDate", "TotalAssets", "TotalLiabilities", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsDeleted", "NetWorth", "Notes", "SnapshotDate", "TotalAssets", "TotalLiabilities", "UpdatedAt", "UserId"
FROM "net_worth_snapshots";

CREATE TABLE "ef_temp_InAppNotifications" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_InAppNotifications" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "IsRead" INTEGER NOT NULL,
    "Message" TEXT NOT NULL,
    "PayloadJson" TEXT NULL,
    "ReadAt" TEXT NULL,
    "Title" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_InAppNotifications_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_InAppNotifications" ("Id", "CreatedAt", "DeletedAt", "IsDeleted", "IsRead", "Message", "PayloadJson", "ReadAt", "Title", "Type", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsDeleted", "IsRead", "Message", "PayloadJson", "ReadAt", "Title", "Type", "UpdatedAt", "UserId"
FROM "InAppNotifications";

CREATE TABLE "ef_temp_FinancialNudges" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_FinancialNudges" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "IsApprovedByUser" INTEGER NULL,
    "IsDeleted" INTEGER NOT NULL,
    "Message" TEXT NOT NULL,
    "NudgeType" INTEGER NOT NULL,
    "RationaleExplanationJson" text NULL,
    "RebalanceProposalJson" text NULL,
    "ResolvedAt" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "Title" TEXT NOT NULL,
    "TriggerEvent" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_FinancialNudges_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_FinancialNudges" ("Id", "CreatedAt", "DeletedAt", "IsApprovedByUser", "IsDeleted", "Message", "NudgeType", "RationaleExplanationJson", "RebalanceProposalJson", "ResolvedAt", "Status", "Title", "TriggerEvent", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsApprovedByUser", "IsDeleted", "Message", "NudgeType", "RationaleExplanationJson", "RebalanceProposalJson", "ResolvedAt", "Status", "Title", "TriggerEvent", "UpdatedAt", "UserId"
FROM "FinancialNudges";

CREATE TABLE "ef_temp_DevicePushTokens" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_DevicePushTokens" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "DeviceId" TEXT NULL,
    "DeviceToken" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "LastActiveAt" TEXT NOT NULL,
    "Platform" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_DevicePushTokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_DevicePushTokens" ("Id", "CreatedAt", "DeletedAt", "DeviceId", "DeviceToken", "IsDeleted", "LastActiveAt", "Platform", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "DeviceId", "DeviceToken", "IsDeleted", "LastActiveAt", "Platform", "UpdatedAt", "UserId"
FROM "DevicePushTokens";

CREATE TABLE "ef_temp_daily_spending_allowances" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_daily_spending_allowances" PRIMARY KEY,
    "ActualSpentToday" decimal(18,2) NOT NULL,
    "BudgetId" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DailyAllowanceAmount" decimal(18,2) NOT NULL,
    "Date" TEXT NOT NULL,
    "OutOfMoneyForecastDate" TEXT NULL,
    "RemainingAllowanceToday" decimal(18,2) NOT NULL,
    "UpdatedAt" TEXT NULL,
    "VelocityIndex" decimal(8,4) NOT NULL DEFAULT '1.0',
    CONSTRAINT "FK_daily_spending_allowances_budgets_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "budgets" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_daily_spending_allowances" ("Id", "ActualSpentToday", "BudgetId", "CreatedAt", "DailyAllowanceAmount", "Date", "OutOfMoneyForecastDate", "RemainingAllowanceToday", "UpdatedAt", "VelocityIndex")
SELECT "Id", "ActualSpentToday", "BudgetId", "CreatedAt", "DailyAllowanceAmount", "Date", "OutOfMoneyForecastDate", "RemainingAllowanceToday", "UpdatedAt", "VelocityIndex"
FROM "daily_spending_allowances";

CREATE TABLE "ef_temp_CoolingOffTransactions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_CoolingOffTransactions" PRIMARY KEY,
    "Amount" TEXT NOT NULL,
    "BypassReason" TEXT NULL,
    "CategoryId" TEXT NOT NULL,
    "CoolingOffUntil" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "Description" TEXT NOT NULL,
    "EquivalentWorkHours" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "ProjectedDsaReduction" TEXT NOT NULL,
    "ResolvedAt" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    "WalletId" TEXT NOT NULL,
    CONSTRAINT "FK_CoolingOffTransactions_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_CoolingOffTransactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CoolingOffTransactions_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_CoolingOffTransactions" ("Id", "Amount", "BypassReason", "CategoryId", "CoolingOffUntil", "CreatedAt", "DeletedAt", "Description", "EquivalentWorkHours", "IsDeleted", "ProjectedDsaReduction", "ResolvedAt", "Status", "UpdatedAt", "UserId", "WalletId")
SELECT "Id", "Amount", "BypassReason", "CategoryId", "CoolingOffUntil", "CreatedAt", "DeletedAt", "Description", "EquivalentWorkHours", "IsDeleted", "ProjectedDsaReduction", "ResolvedAt", "Status", "UpdatedAt", "UserId", "WalletId"
FROM "CoolingOffTransactions";

CREATE TABLE "ef_temp_categories" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_categories" PRIMARY KEY,
    "BudgetRuleMapping" INTEGER NULL,
    "ColorHex" TEXT NOT NULL DEFAULT '#10B981',
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "DisplayOrder" INTEGER NOT NULL DEFAULT 0,
    "IconKey" TEXT NOT NULL DEFAULT 'folder',
    "IncomeGroup" INTEGER NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "IsSystem" INTEGER NOT NULL DEFAULT 0,
    "Name" TEXT NOT NULL,
    "ParentId" TEXT NULL,
    "TransactionType" INTEGER NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NULL,
    CONSTRAINT "FK_categories_categories_ParentId" FOREIGN KEY ("ParentId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_categories_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_categories" ("Id", "BudgetRuleMapping", "ColorHex", "CreatedAt", "DeletedAt", "DisplayOrder", "IconKey", "IncomeGroup", "IsDeleted", "IsSystem", "Name", "ParentId", "TransactionType", "UpdatedAt", "UserId")
SELECT "Id", "BudgetRuleMapping", "ColorHex", "CreatedAt", "DeletedAt", "DisplayOrder", "IconKey", "IncomeGroup", "IsDeleted", "IsSystem", "Name", "ParentId", "TransactionType", "UpdatedAt", "UserId"
FROM "categories";

CREATE TABLE "ef_temp_budgets" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_budgets" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "Method" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "PeriodType" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL
);

INSERT INTO "ef_temp_budgets" ("Id", "CreatedAt", "EndDate", "Method", "Name", "PeriodType", "StartDate", "Status", "TotalAmount", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "EndDate", "Method", "Name", "PeriodType", "StartDate", "Status", "TotalAmount", "UpdatedAt", "UserId"
FROM "budgets";

CREATE TABLE "ef_temp_budget_allocations" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_budget_allocations" PRIMARY KEY,
    "AlertThresholdPercent" decimal(5,2) NOT NULL DEFAULT '80.0',
    "AllocatedAmount" decimal(18,2) NOT NULL,
    "BudgetId" TEXT NOT NULL,
    "CategoryId" TEXT NOT NULL,
    "CurrentSpent" decimal(18,2) NOT NULL,
    CONSTRAINT "FK_budget_allocations_budgets_BudgetId" FOREIGN KEY ("BudgetId") REFERENCES "budgets" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_budget_allocations_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT
);

INSERT INTO "ef_temp_budget_allocations" ("Id", "AlertThresholdPercent", "AllocatedAmount", "BudgetId", "CategoryId", "CurrentSpent")
SELECT "Id", "AlertThresholdPercent", "AllocatedAmount", "BudgetId", "CategoryId", "CurrentSpent"
FROM "budget_allocations";

CREATE TABLE "ef_temp_balance_adjustments" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_balance_adjustments" PRIMARY KEY,
    "AdjustmentDate" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "DifferenceAmount" TEXT NOT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "NewBalance" TEXT NOT NULL,
    "OldBalance" TEXT NOT NULL,
    "ProofImageUrl" TEXT NULL,
    "Reason" TEXT NULL,
    "UpdatedAt" TEXT NULL,
    "WalletId" TEXT NOT NULL,
    CONSTRAINT "FK_balance_adjustments_wallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES "wallets" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_balance_adjustments" ("Id", "AdjustmentDate", "CreatedAt", "DeletedAt", "DifferenceAmount", "IsDeleted", "NewBalance", "OldBalance", "ProofImageUrl", "Reason", "UpdatedAt", "WalletId")
SELECT "Id", "AdjustmentDate", "CreatedAt", "DeletedAt", "DifferenceAmount", "IsDeleted", "NewBalance", "OldBalance", "ProofImageUrl", "Reason", "UpdatedAt", "WalletId"
FROM "balance_adjustments";

CREATE TABLE "ef_temp_auto_categorization_rules" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_auto_categorization_rules" PRIMARY KEY,
    "CreatedAt" TEXT NOT NULL,
    "DeletedAt" TEXT NULL,
    "IsCaseSensitive" INTEGER NOT NULL DEFAULT 0,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    "IsRegex" INTEGER NOT NULL DEFAULT 0,
    "KeywordPattern" TEXT NOT NULL,
    "PriorityOrder" INTEGER NOT NULL DEFAULT 0,
    "TargetCategoryId" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "FK_auto_categorization_rules_categories_TargetCategoryId" FOREIGN KEY ("TargetCategoryId") REFERENCES "categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_auto_categorization_rules_users_UserId" FOREIGN KEY ("UserId") REFERENCES "users" ("Id") ON DELETE CASCADE
);

INSERT INTO "ef_temp_auto_categorization_rules" ("Id", "CreatedAt", "DeletedAt", "IsCaseSensitive", "IsDeleted", "IsRegex", "KeywordPattern", "PriorityOrder", "TargetCategoryId", "UpdatedAt", "UserId")
SELECT "Id", "CreatedAt", "DeletedAt", "IsCaseSensitive", "IsDeleted", "IsRegex", "KeywordPattern", "PriorityOrder", "TargetCategoryId", "UpdatedAt", "UserId"
FROM "auto_categorization_rules";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "WalletTransfers";

ALTER TABLE "ef_temp_WalletTransfers" RENAME TO "WalletTransfers";

DROP TABLE "wallets";

ALTER TABLE "ef_temp_wallets" RENAME TO "wallets";

DROP TABLE "users";

ALTER TABLE "ef_temp_users" RENAME TO "users";

DROP TABLE "user_otp_codes";

ALTER TABLE "ef_temp_user_otp_codes" RENAME TO "user_otp_codes";

DROP TABLE "transactions";

ALTER TABLE "ef_temp_transactions" RENAME TO "transactions";

DROP TABLE "savings_goals";

ALTER TABLE "ef_temp_savings_goals" RENAME TO "savings_goals";

DROP TABLE "savings_goal_contributions";

ALTER TABLE "ef_temp_savings_goal_contributions" RENAME TO "savings_goal_contributions";

DROP TABLE "salary_slip_details";

ALTER TABLE "ef_temp_salary_slip_details" RENAME TO "salary_slip_details";

DROP TABLE "refresh_tokens";

ALTER TABLE "ef_temp_refresh_tokens" RENAME TO "refresh_tokens";

DROP TABLE "recurring_incomes";

ALTER TABLE "ef_temp_recurring_incomes" RENAME TO "recurring_incomes";

DROP TABLE "receipt_items";

ALTER TABLE "ef_temp_receipt_items" RENAME TO "receipt_items";

DROP TABLE "net_worth_snapshots";

ALTER TABLE "ef_temp_net_worth_snapshots" RENAME TO "net_worth_snapshots";

DROP TABLE "InAppNotifications";

ALTER TABLE "ef_temp_InAppNotifications" RENAME TO "InAppNotifications";

DROP TABLE "FinancialNudges";

ALTER TABLE "ef_temp_FinancialNudges" RENAME TO "FinancialNudges";

DROP TABLE "DevicePushTokens";

ALTER TABLE "ef_temp_DevicePushTokens" RENAME TO "DevicePushTokens";

DROP TABLE "daily_spending_allowances";

ALTER TABLE "ef_temp_daily_spending_allowances" RENAME TO "daily_spending_allowances";

DROP TABLE "CoolingOffTransactions";

ALTER TABLE "ef_temp_CoolingOffTransactions" RENAME TO "CoolingOffTransactions";

DROP TABLE "categories";

ALTER TABLE "ef_temp_categories" RENAME TO "categories";

DROP TABLE "budgets";

ALTER TABLE "ef_temp_budgets" RENAME TO "budgets";

DROP TABLE "budget_allocations";

ALTER TABLE "ef_temp_budget_allocations" RENAME TO "budget_allocations";

DROP TABLE "balance_adjustments";

ALTER TABLE "ef_temp_balance_adjustments" RENAME TO "balance_adjustments";

DROP TABLE "auto_categorization_rules";

ALTER TABLE "ef_temp_auto_categorization_rules" RENAME TO "auto_categorization_rules";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;
CREATE INDEX "IX_WalletTransfers_DestinationWalletId" ON "WalletTransfers" ("DestinationWalletId");

CREATE INDEX "IX_WalletTransfers_SourceWalletId" ON "WalletTransfers" ("SourceWalletId");

CREATE INDEX "IX_WalletTransfers_UserId_TransferDate" ON "WalletTransfers" ("UserId", "TransferDate");

CREATE INDEX "IX_wallets_UserId_Status" ON "wallets" ("UserId", "Status");

CREATE UNIQUE INDEX "IX_users_Email" ON "users" ("Email");

CREATE INDEX "IX_user_otp_codes_Email_Purpose_IsUsed" ON "user_otp_codes" ("Email", "Purpose", "IsUsed");

CREATE INDEX "IX_user_otp_codes_ExpiredAt" ON "user_otp_codes" ("ExpiredAt");

CREATE INDEX "IX_user_otp_codes_UserId" ON "user_otp_codes" ("UserId");

CREATE INDEX "IX_transactions_CategoryId_IsDeleted" ON "transactions" ("CategoryId", "IsDeleted");

CREATE INDEX "IX_transactions_UserId_TransactionDate_IsDeleted" ON "transactions" ("UserId", "TransactionDate", "IsDeleted");

CREATE INDEX "IX_transactions_UserId_TransactionType_TransactionDate" ON "transactions" ("UserId", "TransactionType", "TransactionDate");

CREATE INDEX "IX_transactions_WalletId_IsDeleted" ON "transactions" ("WalletId", "IsDeleted");

CREATE INDEX "IX_savings_goals_UserId_Status" ON "savings_goals" ("UserId", "Status");

CREATE INDEX "IX_savings_goals_UserId_TargetDate" ON "savings_goals" ("UserId", "TargetDate");

CREATE INDEX "IX_savings_goal_contributions_SavingsGoalId_ContributionDate" ON "savings_goal_contributions" ("SavingsGoalId", "ContributionDate");

CREATE INDEX "IX_savings_goal_contributions_TransactionId" ON "savings_goal_contributions" ("TransactionId");

CREATE INDEX "IX_savings_goal_contributions_WalletId" ON "savings_goal_contributions" ("WalletId");

CREATE UNIQUE INDEX "IX_salary_slip_details_TransactionId" ON "salary_slip_details" ("TransactionId");

CREATE INDEX "IX_salary_slip_details_UserId_PayPeriod" ON "salary_slip_details" ("UserId", "PayPeriod");

CREATE INDEX "IX_refresh_tokens_Token" ON "refresh_tokens" ("Token");

CREATE INDEX "IX_refresh_tokens_UserId" ON "refresh_tokens" ("UserId");

CREATE INDEX "IX_recurring_incomes_CategoryId" ON "recurring_incomes" ("CategoryId");

CREATE INDEX "IX_recurring_incomes_IsActive_AutoRecord_NextDueDate" ON "recurring_incomes" ("IsActive", "AutoRecord", "NextDueDate");

CREATE INDEX "IX_recurring_incomes_UserId_IsDeleted" ON "recurring_incomes" ("UserId", "IsDeleted");

CREATE INDEX "IX_recurring_incomes_WalletId" ON "recurring_incomes" ("WalletId");

CREATE INDEX "IX_receipt_items_CategoryId" ON "receipt_items" ("CategoryId");

CREATE INDEX "IX_receipt_items_TransactionId" ON "receipt_items" ("TransactionId");

CREATE INDEX "ix_net_worth_snapshots_user_date" ON "net_worth_snapshots" ("UserId", "SnapshotDate");

CREATE INDEX "IX_InAppNotifications_UserId" ON "InAppNotifications" ("UserId");

CREATE INDEX "IX_FinancialNudges_UserId_NudgeType_CreatedAt" ON "FinancialNudges" ("UserId", "NudgeType", "CreatedAt");

CREATE INDEX "IX_FinancialNudges_UserId_Status" ON "FinancialNudges" ("UserId", "Status");

CREATE INDEX "IX_DevicePushTokens_UserId" ON "DevicePushTokens" ("UserId");

CREATE UNIQUE INDEX "IX_daily_spending_allowances_BudgetId_Date" ON "daily_spending_allowances" ("BudgetId", "Date");

CREATE INDEX "IX_CoolingOffTransactions_CategoryId" ON "CoolingOffTransactions" ("CategoryId");

CREATE INDEX "IX_CoolingOffTransactions_UserId_CoolingOffUntil" ON "CoolingOffTransactions" ("UserId", "CoolingOffUntil");

CREATE INDEX "IX_CoolingOffTransactions_UserId_Status" ON "CoolingOffTransactions" ("UserId", "Status");

CREATE INDEX "IX_CoolingOffTransactions_WalletId" ON "CoolingOffTransactions" ("WalletId");

CREATE INDEX "IX_categories_IsSystem" ON "categories" ("IsSystem");

CREATE INDEX "IX_categories_ParentId" ON "categories" ("ParentId");

CREATE INDEX "IX_categories_UserId_TransactionType_IsDeleted" ON "categories" ("UserId", "TransactionType", "IsDeleted");

CREATE INDEX "IX_budgets_UserId_StartDate_EndDate" ON "budgets" ("UserId", "StartDate", "EndDate");

CREATE INDEX "IX_budgets_UserId_Status" ON "budgets" ("UserId", "Status");

CREATE UNIQUE INDEX "IX_budget_allocations_BudgetId_CategoryId" ON "budget_allocations" ("BudgetId", "CategoryId");

CREATE INDEX "IX_budget_allocations_CategoryId" ON "budget_allocations" ("CategoryId");

CREATE INDEX "IX_balance_adjustments_WalletId_AdjustmentDate" ON "balance_adjustments" ("WalletId", "AdjustmentDate");

CREATE INDEX "IX_auto_categorization_rules_TargetCategoryId" ON "auto_categorization_rules" ("TargetCategoryId");

CREATE INDEX "IX_auto_categorization_rules_UserId_KeywordPattern" ON "auto_categorization_rules" ("UserId", "KeywordPattern");

CREATE INDEX "IX_auto_categorization_rules_UserId_PriorityOrder" ON "auto_categorization_rules" ("UserId", "PriorityOrder");

COMMIT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261101123941_AddDebtsAndSubscriptionsTables', '10.0.13');

