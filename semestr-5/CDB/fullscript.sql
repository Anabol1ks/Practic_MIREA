-- Таблица Client
CREATE TABLE client (
    client_id SERIAL PRIMARY KEY,
    full_name_id INT NOT NULL,
    birth_date DATE,
    phone_number VARCHAR(20) NOT NULL,
    email VARCHAR(50) UNIQUE NOT NULL
);

-- Таблица PC
CREATE TABLE pc (
    pc_id SERIAL PRIMARY KEY,
    model VARCHAR(50) NOT NULL,
    ram_size INT NOT NULL,
    cpu_model VARCHAR(50) NOT NULL,
    gpu_model VARCHAR(50) NOT NULL,
    purchase_date DATE NOT NULL
);

-- Таблица Workstation
CREATE TABLE workstation (
    workstation_id SERIAL PRIMARY KEY,
    pc_id INT NOT NULL REFERENCES pc(pc_id),
    location VARCHAR(50) NOT NULL
);

-- Таблица Session
CREATE TABLE session (
    session_id SERIAL PRIMARY KEY,
    client_id INT NOT NULL REFERENCES client(client_id),
    workstation_id INT NOT NULL REFERENCES workstation(workstation_id),
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL
);

-- Таблица Tariff
CREATE TABLE tariff (
    tariff_id SERIAL PRIMARY KEY,
    tariff_name VARCHAR(50) NOT NULL,
    price_per_hour DECIMAL NOT NULL
);

-- Таблица Payment
CREATE TABLE payment (
    payment_id SERIAL PRIMARY KEY,
    session_id INT NOT NULL REFERENCES session(session_id),
    tariff_id INT NOT NULL REFERENCES tariff(tariff_id),
    payment_amount DECIMAL NOT NULL,
    payment_time TIMESTAMP NOT NULL
);

-- Таблица Staff
CREATE TABLE staff (
    staff_id SERIAL PRIMARY KEY,
    full_name_id INT NOT NULL,
    position_id INT NOT NULL,
    hire_date DATE NOT NULL
);

-- Таблица PC_Service
CREATE TABLE pc_service (
    service_id SERIAL PRIMARY KEY,
    pc_id INT NOT NULL REFERENCES pc(pc_id),
    staff_id INT NOT NULL REFERENCES staff(staff_id),
    service_date DATE NOT NULL,
    service_type VARCHAR(50) NOT NULL,
    comments TEXT
);

-- Словарь Full_Name
CREATE TABLE full_name (
    full_name_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL
);

-- Словарь Position
CREATE TABLE position (
    position_id SERIAL PRIMARY KEY,
    name_position VARCHAR(50) NOT NULL
);


ALTER TABLE Staff
ADD FOREIGN KEY (full_name_id) REFERENCES Full_Name(full_name_id),
ADD FOREIGN KEY (position_id) REFERENCES Position(position_id);

ALTER TABLE Client
ADD FOREIGN KEY (full_name_id) REFERENCES Full_Name(full_name_id);

INSERT INTO full_name (first_name, last_name)
VALUES
('John', 'Doe'), ('Jane', 'Smith'), ('Sam', 'Brown'), 
('Alex', 'Johnson'), ('Chris', 'Lee'), ('Emily', 'Davis'),
('Michael', 'Miller'), ('Sara', 'Wilson'), ('David', 'Moore'),
('Laura', 'Taylor');

INSERT INTO client (full_name_id, birth_date, phone_number, email)
VALUES
(1, '1985-05-12', '1234567890', 'john.doe@example.com'),
(2, '1990-06-22', '0987654321', 'jane.smith@example.com'),
(3, '1988-03-15', '1122334455', 'sam.brown@example.com'),
(4, '1995-11-30', '5566778899', 'alex.johnson@example.com'),
(5, '1980-12-25', '6677889900', 'chris.lee@example.com'),
(6, '1992-08-18', '7788990011', 'emily.davis@example.com'),
(7, '1983-01-05', '8899001122', 'michael.miller@example.com'),
(8, '1997-07-20', '9900112233', 'sara.wilson@example.com'),
(9, '1981-09-14', '1122445566', 'david.moore@example.com'),
(10, '1999-10-05', '2233556677', 'laura.taylor@example.com');

INSERT INTO pc (model, ram_size, cpu_model, gpu_model, purchase_date)
VALUES
('PC1', 16, 'Intel i5', 'NVIDIA GTX 1650', '2022-01-01'),
('PC2', 32, 'AMD Ryzen 7', 'NVIDIA RTX 3060', '2023-02-10'),
('PC3', 16, 'Intel i7', 'NVIDIA GTX 1660', '2022-03-15'),
('PC4', 8, 'AMD Ryzen 5', 'NVIDIA GTX 1050', '2021-07-25'),
('PC5', 16, 'Intel i5', 'AMD RX 580', '2022-05-30'),
('PC6', 32, 'Intel i9', 'NVIDIA RTX 3080', '2023-06-12'),
('PC7', 16, 'AMD Ryzen 9', 'NVIDIA RTX 3070', '2023-07-21'),
('PC8', 8, 'Intel i3', 'NVIDIA GTX 950', '2021-09-10'),
('PC9', 16, 'AMD Ryzen 3', 'NVIDIA GTX 1060', '2020-11-15'),
('PC10', 32, 'Intel i7', 'NVIDIA RTX 2070', '2022-12-31');

INSERT INTO workstation (pc_id, location)
VALUES
(1, 'Room 101'), (2, 'Room 102'), (3, 'Room 103'), 
(4, 'Room 104'), (5, 'Room 105'), (6, 'Room 106'),
(7, 'Room 107'), (8, 'Room 108'), (9, 'Room 109'), 
(10, 'Room 110');

INSERT INTO session (client_id, workstation_id, start_time, end_time)
VALUES
(1, 2, '2024-09-30 10:00:00', '2024-09-30 12:00:00'),
(2, 4, '2024-09-30 11:00:00', '2024-09-30 13:00:00'),
(3, 3, '2024-09-30 14:00:00', '2024-09-30 16:00:00'),
(4, 5, '2024-10-01 09:00:00', '2024-10-01 11:00:00'),
(5, 6, '2024-10-01 13:00:00', '2024-10-01 15:00:00'),
(6, 7, '2024-10-02 10:00:00', '2024-10-02 12:00:00'),
(7, 8, '2024-10-02 11:00:00', '2024-10-02 13:00:00'),
(8, 9, '2024-10-02 14:00:00', '2024-10-02 16:00:00'),
(9, 1, '2024-10-03 10:00:00', '2024-10-03 12:00:00'),
(10, 10, '2024-10-03 14:00:00', '2024-10-03 16:00:00');

INSERT INTO tariff (tariff_name, price_per_hour)
VALUES
('Standard', 5.00), ('Premium', 10.00), ('VIP', 15.00),
('Economy', 3.00), ('Basic', 2.00), ('Advanced', 8.00),
('Deluxe', 12.00), ('Pro', 6.00), ('Elite', 18.00), 
('Student', 4.00);

INSERT INTO payment (session_id, tariff_id, payment_amount, payment_time)
VALUES
(3, 1, 15.00, '2024-09-30 12:00:00'), (5, 3, 30.00, '2024-09-30 13:00:00'), 
(7, 2, 20.00, '2024-09-30 14:00:00'), (4, 5, 10.00, '2024-10-01 11:00:00'), 
(1, 7, 35.00, '2024-10-01 15:00:00'), (6, 9, 45.00, '2024-10-02 12:00:00'), 
(2, 4, 8.00, '2024-10-02 16:00:00'), (8, 6, 24.00, '2024-10-02 12:00:00'), 
(10, 8, 40.00, '2024-10-03 12:00:00'), (9, 10, 50.00, '2024-10-03 16:00:00');

INSERT INTO position (name_position)
VALUES
('Technician'), ('Manager'), ('Developer'), ('Support'),
('Network Administrator'), ('Security Specialist'),
('System Analyst'), ('Database Administrator'),
('Hardware Engineer'), ('Helpdesk Specialist');

INSERT INTO staff (full_name_id, position_id, hire_date)
VALUES
(1, 1, '2023-01-01'), (2, 2, '2022-05-10'), (3, 3, '2021-03-15'),
(4, 4, '2023-07-20'), (5, 5, '2021-11-25'), (6, 6, '2020-09-13'),
(7, 7, '2019-12-30'), (8, 8, '2022-04-22'), (9, 9, '2023-03-03'),
(10, 10, '2021-06-18');


INSERT INTO pc_service (pc_id, staff_id, service_date, service_type, comments)
VALUES
(1, 1, '2024-09-15', 'Repair', 'Replaced faulty RAM'),
(2, 2, '2024-09-18', 'Upgrade', 'Upgraded to 32GB RAM'),
(3, 3, '2024-09-20', 'Maintenance', 'Cleaned the fans and applied new thermal paste'),
(4, 4, '2024-09-22', 'Repair', 'Replaced GPU'),
(5, 5, '2024-09-25', 'Maintenance', 'Routine check-up'),
(6, 6, '2024-09-27', 'Repair', 'Fixed power supply issue'),
(7, 7, '2024-09-29', 'Upgrade', 'Upgraded to SSD storage'),
(8, 8, '2024-10-01', 'Maintenance', 'Cleaned and applied new thermal paste'),
(9, 9, '2024-10-03', 'Repair', 'Replaced CPU cooler'),
(10, 10, '2024-10-05', 'Maintenance', 'Routine cleaning and check-up');


-- Содержимое таблицы Client
SELECT * FROM client;

-- Содержимое таблицы PC
SELECT * FROM pc;

-- Содержимое таблицы Workstation
SELECT * FROM workstation;

-- Содержимое таблицы Session
SELECT * FROM session;

-- Содержимое таблицы Tariff
SELECT * FROM tariff;

-- Содержимое таблицы Payment
SELECT * FROM payment;

-- Содержимое таблицы Staff
SELECT * FROM staff;

-- Содержимое таблицы PC_Service
SELECT * FROM pc_service;

-- Содержимое таблицы Full_Name (словарь)
SELECT * FROM full_name;

SELECT * FROM position;

SELECT * FROM Client
WHERE client_id = 4

SELECT * FROM Client
WHERE client_id > 4

SELECT * FROM Client
WHERE client_id < 4

SELECT * FROM Client
WHERE client_id >= 4

SELECT * FROM Client
WHERE client_id <= 4

SELECT * FROM Client
WHERE client_id != 4

SELECT * FROM Client
WHERE birth_date IS NOT NULL

SELECT * FROM Client
WHERE email IS NULL

SELECT * FROM PC
WHERE ram_size IN(8,32)

SELECT * FROM PC
WHERE ram_size NOT IN(8,32)

SELECT * FROM PC
WHERE cpu_model LIKE '%AMD%'

SELECT * FROM PC
WHERE cpu_model NOT LIKE '%AMD%'
