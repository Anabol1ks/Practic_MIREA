CREATE TABLE "Client"(
    "client_id" BIGINT NOT NULL,
    "name" VARCHAR(255) NOT NULL,
    "surename" VARCHAR(255) NOT NULL,
    "patronim" VARCHAR(255) NOT NULL,
    "phone" VARCHAR(255) NOT NULL,
    "email" VARCHAR(255) NULL
);
ALTER TABLE
    "Client" ADD PRIMARY KEY("client_id");
CREATE TABLE "PC"(
    "Pc_id" BIGINT NOT NULL,
    "is_working" BOOLEAN NOT NULL
);
ALTER TABLE
    "PC" ADD PRIMARY KEY("Pc_id");
CREATE TABLE "PC_Club"(
    "club_id" BIGINT NOT NULL,
    "name" VARCHAR(255) NOT NULL,
    "address" VARCHAR(255) NOT NULL,
    "is_working" BOOLEAN NOT NULL
);
ALTER TABLE
    "PC_Club" ADD PRIMARY KEY("club_id");
CREATE TABLE "Staff"(
    "staff_id" BIGINT NOT NULL,
    "club_id" BIGINT NOT NULL,
    "position" VARCHAR(255) NOT NULL,
    "phone" VARCHAR(255) NOT NULL,
    "email" VARCHAR(255) NOT NULL
);
ALTER TABLE
    "Staff" ADD PRIMARY KEY("staff_id");
CREATE TABLE "Order"(
    "order_id" BIGINT NOT NULL,
    "staff_id" BIGINT NOT NULL,
    "client_id" BIGINT NOT NULL,
    "order_date" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
    "duration" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
    "pc_id" BIGINT NOT NULL
);
ALTER TABLE
    "Order" ADD PRIMARY KEY("order_id");
CREATE TABLE "Status"(
    "status_id" BIGINT NOT NULL,
    "status_name" VARCHAR(255) NOT NULL
);
ALTER TABLE
    "Status" ADD PRIMARY KEY("status_id");
CREATE TABLE "OrderStatus"(
    "order_status_id" BIGINT NOT NULL,
    "tariff_id" BIGINT NOT NULL,
    "status_id" BIGINT NOT NULL,
    "date_start" TIMESTAMP(0) WITHOUT TIME ZONE NOT NULL,
    "date_end" TIMESTAMP(0) WITHOUT TIME ZONE NULL
);
ALTER TABLE
    "OrderStatus" ADD PRIMARY KEY("order_status_id");
CREATE TABLE "Software"(
    "software_id" BIGINT NOT NULL,
    "software_name" VARCHAR(255) NOT NULL,
    "version" VARCHAR(255) NOT NULL
);
ALTER TABLE
    "Software" ADD PRIMARY KEY("software_id");
CREATE TABLE "OS"(
    "os_id" BIGINT NOT NULL,
    "os_name" VARCHAR(255) NOT NULL,
    "os_version" VARCHAR(255) NOT NULL
);
ALTER TABLE
    "OS" ADD PRIMARY KEY("os_id");
CREATE TABLE "PC_OS"(
    "PC_OS_id" BIGINT NOT NULL,
    "PC_id" BIGINT NOT NULL,
    "OS_id" BIGINT NOT NULL,
    "instalation_date" DATE NOT NULL
);
ALTER TABLE
    "PC_OS" ADD PRIMARY KEY("PC_OS_id");
CREATE TABLE "PC_Software"(
    "PC_Software_id" BIGINT NOT NULL,
    "PC_id" BIGINT NOT NULL,
    "software_id" BIGINT NOT NULL,
    "instalation_date" DATE NOT NULL
);
ALTER TABLE
    "PC_Software" ADD PRIMARY KEY("PC_Software_id");
ALTER TABLE
    "PC_OS" ADD CONSTRAINT "pc_os_os_id_foreign" FOREIGN KEY("OS_id") REFERENCES "OS"("os_id");
ALTER TABLE
    "PC_Software" ADD CONSTRAINT "pc_software_pc_id_foreign" FOREIGN KEY("PC_id") REFERENCES "PC"("Pc_id");
ALTER TABLE
    "Staff" ADD CONSTRAINT "staff_club_id_foreign" FOREIGN KEY("club_id") REFERENCES "PC_Club"("club_id");
ALTER TABLE
    "Order" ADD CONSTRAINT "order_staff_id_foreign" FOREIGN KEY("staff_id") REFERENCES "Staff"("staff_id");
ALTER TABLE
    "OrderStatus" ADD CONSTRAINT "orderstatus_tariff_id_foreign" FOREIGN KEY("tariff_id") REFERENCES "Order"("order_id");
ALTER TABLE
    "PC_OS" ADD CONSTRAINT "pc_os_pc_id_foreign" FOREIGN KEY("PC_id") REFERENCES "PC"("Pc_id");
ALTER TABLE
    "Order" ADD CONSTRAINT "order_client_id_foreign" FOREIGN KEY("client_id") REFERENCES "Client"("client_id");
ALTER TABLE
    "PC_Software" ADD CONSTRAINT "pc_software_software_id_foreign" FOREIGN KEY("software_id") REFERENCES "Software"("software_id");
ALTER TABLE
    "Order" ADD CONSTRAINT "order_pc_id_foreign" FOREIGN KEY("pc_id") REFERENCES "PC"("Pc_id");
ALTER TABLE
    "OrderStatus" ADD CONSTRAINT "orderstatus_status_id_foreign" FOREIGN KEY("status_id") REFERENCES "Status"("status_id");