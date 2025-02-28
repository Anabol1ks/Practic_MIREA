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


CREATE TABLE tar (
    tariff_id SERIAL PRIMARY KEY,
    tariff_name VARCHAR(50) NOT NULL,
    price_per_hour DECIMAL NOT NULL
);

INSERT INTO tar (tariff_name, price_per_hour) 
VALUES
('Standard', 5.00), 
('Premium', 10.00), 
('VIP', 15.00), 
('Economy', 3.00), 
('Elite', 18.00);

ALTER TABLE tar ADD COLUMN bonus numeric;

UPDATE tar
SET bonus = CASE
    WHEN tariff_id = 1 THEN 1.5
    WHEN tariff_id = 2 THEN 2.0
    WHEN tariff_id = 3 THEN 2.5
    WHEN tariff_id = 4 THEN 1.0
    WHEN tariff_id = 5 THEN 0.5
END
WHERE tariff_id IN (1, 2, 3, 4, 5);

SELECT * FROM tar;

UPDATE tar
SET bonus = 10
WHERE tariff_id IN (3, 5);

ALTER TABLE tar
RENAME COLUMN tariff_name TO tariff_plan;


DELETE FROM tar
WHERE tariff_id IN (2, 4);


SELECT * FROM Workstation
WHERE pc_id > 4 AND pc_id < 6;

CREATE TABLE c (
    client_id SERIAL PRIMARY KEY,
    full_name_id INT NOT NULL,
    birth_date DATE,
    phone_number VARCHAR(20) NOT NULL,
    email VARCHAR(50) UNIQUE NOT NULL
);

INSERT INTO c (full_name_id, birth_date, phone_number, email)
VALUES
(1, '1985-05-12', '1234567890', 'john.doe@example.com'),
(3, '1988-03-15', '1122334455', 'sam.brown@example.com'),
(5, '1980-12-25', '6677889900', 'chris.lee@example.com'),
(7, '1983-01-05', '8899001122', 'michael.miller@example.com'),
(9, '1981-09-14', '1122445566', 'david.moore@example.com');

SELECT * FROM client
INNER JOIN c ON client.full_name_id = c.full_name_id;


SELECT * FROM client
RIGHT JOIN c ON client.full_name_id = c.full_name_id;


SELECT * FROM client
LEFT JOIN c ON client.full_name_id = c.full_name_id;


1. Операция UNION
Соединяет результаты двух запросов и исключает дубликаты.


SELECT full_name_id, birth_date, phone_number, email FROM client
UNION
SELECT full_name_id, birth_date, phone_number, email FROM c;
Если тебе нужно сохранить дубликаты, используй UNION ALL:



SELECT full_name_id, birth_date, phone_number, email FROM client
UNION ALL
SELECT full_name_id, birth_date, phone_number, email FROM c;
2. Операция EXISTS
Проверяет наличие строк в подзапросе.



SELECT *
FROM client
WHERE EXISTS (
    SELECT 1 FROM c WHERE client.full_name_id = c.full_name_id
);
Этот запрос вернёт строки из таблицы client, если соответствующие строки существуют в таблице c.

3. Операция NOT EXISTS
Возвращает строки, где нет соответствующих строк в другой таблице.



SELECT *
FROM client
WHERE NOT EXISTS (
    SELECT 1 FROM c WHERE client.full_name_id = c.full_name_id
);
Этот запрос вернёт строки из таблицы client, для которых не существует соответствующих строк в таблице c.

4. Операции MAX(), MIN(), AVG(), SUM()
SELECT 
	MAX(birth_date) max_birth_date, 
	MIN(birth_date) AS min_birth_date, 
	AVG(EXTRACT(YEAR FROM AGE(birth_date))) AS avg_age, 
	SUM(EXTRACT(YEAR FROM AGE(birth_date))) AS total_years
FROM client;

5. Операция COUNT

SELECT COUNT(*) AS total_clients FROM client
WHERE client_id >=3

6. Операция GROUP BY


SELECT EXTRACT(YEAR FROM birth_date) AS birth_year, COUNT(*) AS count_clients
FROM client
GROUP BY EXTRACT(YEAR FROM birth_date);
Этот запрос сгруппирует строки по году рождения и покажет количество клиентов, родившихся в каждый год.

7. Операция HAVING


SELECT EXTRACT(YEAR FROM birth_date) AS birth_year, COUNT(*) AS count_clients
FROM client
GROUP BY EXTRACT(YEAR FROM birth_date)
HAVING COUNT(*) > 1;
Этот запрос вернёт только те группы, где больше одного клиента родились в одном году.

8. Операция ORDER BY


SELECT * FROM client
ORDER BY birth_date DESC;
Этот запрос отсортирует клиентов по дате рождения в обратном порядке (от самого позднего к самому раннему).

9. Операция деления

INSERT INTO session (client_id, workstation_id, start_time, end_time)
VALUES
(1, 1, '2024-10-05 09:00:00', '2024-10-05 11:00:00'), 
(1, 4, '2024-10-05 13:00:00', '2024-10-05 15:00:00'),
(1, 3, '2024-10-05 11:00:00', '2024-10-05 13:00:00'),
(1, 5, '2024-10-05 15:00:00', '2024-10-05 17:00:00'),
(1, 6, '2024-10-05 17:00:00', '2024-10-05 19:00:00'),
(1, 7, '2024-10-05 19:00:00', '2024-10-05 21:00:00'),
(1, 8, '2024-10-05 21:00:00', '2024-10-05 23:00:00'),
(1, 9, '2024-10-05 23:00:00', '2024-10-06 01:00:00'),
(1, 10, '2024-10-06 01:00:00', '2024-10-06 03:00:00'); 

SELECT client_id
FROM session
GROUP BY client_id
HAVING COUNT(DISTINCT workstation_id) = (SELECT COUNT(*) FROM workstation);
Этот запрос вернет клиента, который использовал все доступные рабочие станции.

Проверь этот запрос после добавления сессий — он должен вернуть клиента с client_id = 1


-- Выборка 
-- Хранимые процедуры
CREATE OR REPLACE FUNCTION GetClientSessions(client_id INT)
RETURNS TABLE(
    session_id INT,
    location VARCHAR,
    tariff_name VARCHAR,
    price_per_hour DECIMAL,
    start_time TIMESTAMP,
    end_time TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT s.session_id, w.location, t.tariff_name, t.price_per_hour, s.start_time, s.end_time
    FROM session s
    JOIN workstation w ON s.workstation_id = w.workstation_id
    JOIN tariff t ON EXISTS (
        SELECT 1
        FROM payment p
        WHERE p.session_id = s.session_id AND p.tariff_id = t.tariff_id
    )
    WHERE s.client_id = GetClientSessions.client_id; -- Явно указываем параметр
END;
$$;


SELECT * FROM GetClientSessions(1);


-- функции
CREATE OR REPLACE FUNCTION GetFullName(full_name_id INT)
RETURNS TEXT
LANGUAGE plpgsql
AS $$
DECLARE
    full_name TEXT;
BEGIN
    SELECT CONCAT(first_name, ' ', last_name)
    INTO full_name
    FROM full_name
    WHERE full_name.full_name_id = GetFullName.full_name_id; 
    RETURN full_name;
END;
$$;


SELECT GetFullName(1);


-- триггер
CREATE OR REPLACE FUNCTION UpdateClientTotalPayment()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE client
    SET total_payment = COALESCE(total_payment, 0) + NEW.payment_amount
    WHERE client_id = NEW.client_id;

    RETURN NEW;
END;
$$;

CREATE TRIGGER AfterPaymentInsert
AFTER INSERT ON payment
FOR EACH ROW
EXECUTE FUNCTION UpdateClientTotalPayment();

ALTER TABLE payment ADD COLUMN client_id INT;

ALTER TABLE payment ADD CONSTRAINT fk_client_id FOREIGN KEY (client_id) REFERENCES client(client_id);


ALTER TABLE client ADD COLUMN total_payment DECIMAL DEFAULT 0;

UPDATE client
SET total_payment = COALESCE((
    SELECT SUM(payment_amount)
    FROM payment
    WHERE payment.client_id = client.client_id
), 0);


INSERT INTO payment (session_id, tariff_id, payment_amount, payment_time, client_id)
VALUES (1, 1, 500.00, NOW(), 1);

SELECT client_id, total_payment FROM client WHERE client_id = 1;



-- ещё процедуры 
CREATE OR REPLACE FUNCTION GetTariffStatistics()
RETURNS TABLE(
    tariff_name VARCHAR,
    total_usage BIGINT,         
    total_revenue NUMERIC,      
    average_revenue NUMERIC     
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        t.tariff_name,
        COUNT(p.payment_id) AS total_usage,
        SUM(p.payment_amount) AS total_revenue,
        AVG(p.payment_amount) AS average_revenue
    FROM tariff t
    LEFT JOIN payment p ON t.tariff_id = p.tariff_id
    GROUP BY t.tariff_name;
END;
$$;

SELECT * FROM GetTariffStatistics();


CREATE OR REPLACE FUNCTION GetStaffSessions(input_staff_id INT)
RETURNS TABLE(
    session_id INT,
    client_id INT,
    client_email VARCHAR,
    workstation_location VARCHAR,
    start_time TIMESTAMP,
    end_time TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        s.session_id,
        c.client_id,
        c.email AS client_email,
        w.location AS workstation_location,
        s.start_time,
        s.end_time
    FROM session s
    JOIN workstation w ON s.workstation_id = w.workstation_id
    JOIN client c ON s.client_id = c.client_id
    WHERE w.pc_id IN (
        SELECT pc_service.pc_id
        FROM pc_service
        WHERE pc_service.staff_id = input_staff_id
    );
END;
$$;


SELECT * FROM GetStaffSessions(1);


-- ещё функции 

CREATE OR REPLACE FUNCTION GetTariffStatistics()
RETURNS TABLE(
    tariff_name VARCHAR,
    total_usage BIGINT,         -- Тип bigint для COUNT
    total_revenue NUMERIC,      -- Тип numeric для SUM
    average_revenue NUMERIC     -- Тип numeric для AVG
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        t.tariff_name,
        COUNT(p.payment_id) AS total_usage,
        SUM(p.payment_amount) AS total_revenue,
        AVG(p.payment_amount) AS average_revenue
    FROM tariff t
    LEFT JOIN payment p ON t.tariff_id = p.tariff_id
    GROUP BY t.tariff_name;
END;
$$;

SELECT * FROM GetTariffStatistics();

CREATE OR REPLACE FUNCTION GetStaffSessions(input_staff_id INT)
RETURNS TABLE(
    session_id INT,
    client_id INT,
    client_email VARCHAR,
    workstation_location VARCHAR,
    start_time TIMESTAMP,
    end_time TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        s.session_id,
        c.client_id,
        c.email AS client_email,
        w.location AS workstation_location,
        s.start_time,
        s.end_time
    FROM session s
    JOIN workstation w ON s.workstation_id = w.workstation_id
    JOIN client c ON s.client_id = c.client_id
    WHERE w.pc_id IN (
        SELECT pc_service.pc_id
        FROM pc_service
        WHERE pc_service.staff_id = input_staff_id
    );
END;
$$;


SELECT * FROM GetStaffSessions(5);


-- Ещё триггеры
-- Функция триггера
CREATE OR REPLACE FUNCTION UpdateLastServiceDate()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE pc
    SET last_service_date = NEW.service_date
    WHERE pc_id = NEW.pc_id;

    RETURN NEW;
END;
$$;

-- Создание триггера
CREATE TRIGGER AfterServiceInsert
AFTER INSERT ON pc_service
FOR EACH ROW
EXECUTE FUNCTION UpdateLastServiceDate();


ALTER TABLE pc ADD COLUMN last_service_date DATE;

INSERT INTO pc_service (pc_id, staff_id, service_date, service_type, comments)
VALUES (1, 2, '2024-12-07', 'Diagnostics', 'Routine check-up');


-- Функция триггера
CREATE OR REPLACE FUNCTION DeleteClientSessions()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    -- Удаляем записи из таблицы payment, связанные с сессиями клиента
    DELETE FROM payment
    WHERE session_id IN (
        SELECT session_id
        FROM session
        WHERE client_id = OLD.client_id
    );

    -- Удаляем сессии клиента
    DELETE FROM session
    WHERE client_id = OLD.client_id;

    RETURN OLD;
END;
$$;

-- Создание триггера
CREATE TRIGGER BeforeClientDelete
BEFORE DELETE ON client
FOR EACH ROW
EXECUTE FUNCTION DeleteClientSessions();



DELETE FROM client WHERE client_id = 1;



-- 6
-- 1.Открытие окна при помощи инструкции «OVER()»
SELECT 
    SUM(payment_amount) OVER () AS total_payment
FROM payment;
-- 2.Применение инструкции «PARTITON BY()»
SELECT 
    SUM(payment_amount) OVER 
	(PARTITION BY client_id) AS client_total_payment
FROM payment;
-- 3.Применение «ORDER BY()»
SELECT 
    payment_time, 
    SUM(payment_amount) OVER (ORDER BY payment_time) AS running_total
FROM payment;
-- 4.Применение агрегатных функций «AVG()» и «COUNT()»
SELECT 
    tariff_id, 
    AVG(payment_amount) OVER (PARTITION BY tariff_id) AS avg_payment,
    COUNT(*) OVER (PARTITION BY tariff_id) AS count_payments
FROM payment;

-- 5.Применение агрегатных функций «SUM()» и «COUNT()»
SELECT 
    SUM(payment_amount) OVER (PARTITION BY client_id) AS total_payment,
    COUNT(*) OVER (PARTITION BY client_id) AS count_payments
FROM payment;
-- 6.Применение агрегатных функций «MAX()» и «COUNT()»
SELECT 
    MAX(payment_amount) OVER (PARTITION BY client_id) AS max_payment,
    COUNT(*) OVER (PARTITION BY client_id) AS count_payments
FROM payment;

-- 7.Применение агрегатных функций «MIN ()» и «COUNT()»
SELECT 
    MIN(payment_amount) OVER (PARTITION BY client_id) AS min_payment,
    COUNT(*) OVER (PARTITION BY client_id) AS count_payments
FROM payment;

-- 8.Применение ранжирующей функции «ROW_NUMBER»
SELECT 
    payment_amount, 
    ROW_NUMBER() OVER (PARTITION BY client_id ORDER BY payment_amount DESC) AS row_num
FROM payment;

-- 9.Применение ранжирующей функции «RANK»
SELECT 
    payment_amount, 
    RANK() OVER (PARTITION BY client_id ORDER BY payment_amount DESC) AS rank
FROM payment;

-- 10.Применение ранжирующей функции «DENSE_RANK»
SELECT 
    payment_amount, 
    DENSE_RANK() OVER (PARTITION BY client_id ORDER BY payment_amount DESC) AS dense_rank
FROM payment;

-- 11.Применение ранжирующей функции «NTILE()»
SELECT 
    payment_amount, 
    NTILE(4) OVER (PARTITION BY client_id ORDER BY payment_amount DESC) AS quartile
FROM payment;

-- 12.Применение функции смещение «LAG()»
SELECT 
    payment_amount, 
    LAG(payment_amount) OVER (PARTITION BY client_id ORDER BY payment_time) AS previous_payment
FROM payment;

-- 13.Применение функции смещение «LEAD()»
SELECT 
    payment_amount, 
    LEAD(payment_amount) OVER (PARTITION BY client_id ORDER BY payment_time) AS next_payment
FROM payment;

-- 14. Применение функции смещение «FIRST_VALUE()»
SELECT 
    payment_amount, 
    FIRST_VALUE(payment_amount) OVER (PARTITION BY client_id ORDER BY payment_time) AS first_payment
FROM payment;

-- 15.Применение функции смещение «LAST_VALUE()»
SELECT 
    payment_amount, 
    LAST_VALUE(payment_amount) OVER (PARTITION BY client_id ORDER BY payment_time ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS last_payment
FROM payment;