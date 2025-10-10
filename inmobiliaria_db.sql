-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 11-10-2025 a las 00:21:33
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria_db`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `InquilinoId` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `FechaInicio` datetime(6) NOT NULL,
  `FechaFin` datetime(6) NOT NULL,
  `MontoAlquiler` decimal(65,30) NOT NULL,
  `FechaRescision` datetime(6) DEFAULT NULL,
  `Multa` decimal(65,30) NOT NULL DEFAULT 0.000000000000000000000000000000,
  `UsuarioIdCreador` int(11) DEFAULT NULL,
  `UsuarioIdTerminador` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `InquilinoId`, `InmuebleId`, `FechaInicio`, `FechaFin`, `MontoAlquiler`, `FechaRescision`, `Multa`, `UsuarioIdCreador`, `UsuarioIdTerminador`) VALUES
(1, 1, 1, '2023-03-01 00:00:00.000000', '2025-02-28 00:00:00.000000', 250000.000000000000000000000000000000, NULL, 0.000000000000000000000000000000, 3, NULL),
(2, 2, 3, '2023-08-15 00:00:00.000000', '2025-08-14 00:00:00.000000', 350000.000000000000000000000000000000, NULL, 0.000000000000000000000000000000, 4, NULL),
(3, 3, 6, '2024-01-10 00:00:00.000000', '2026-01-09 00:00:00.000000', 450000.000000000000000000000000000000, NULL, 0.000000000000000000000000000000, 3, NULL),
(4, 1, 2, '2024-10-26 00:00:00.000000', '2025-10-25 00:00:00.000000', 150000.000000000000000000000000000000, NULL, 0.000000000000000000000000000000, 4, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `Direccion` longtext NOT NULL,
  `Uso` longtext NOT NULL,
  `TipoInmuebleId` int(11) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Precio` decimal(65,30) NOT NULL,
  `Coordenadas` longtext NOT NULL,
  `Disponible` tinyint(1) NOT NULL,
  `PropietarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `Direccion`, `Uso`, `TipoInmuebleId`, `Ambientes`, `Precio`, `Coordenadas`, `Disponible`, `PropietarioId`) VALUES
(1, 'Av. Illia 123, San Luis', 'Residencial', 1, 4, 250000.000000000000000000000000000000, '-33.2974,-66.3357', 0, 1),
(2, 'Junin 456, San Luis', 'Residencial', 2, 2, 150000.000000000000000000000000000000, '-33.3014,-66.3387', 0, 1),
(3, 'Los Paraisos 789, Potrero de los Funes', 'Comercial', 5, 3, 350000.000000000000000000000000000000, '-33.2208,-66.2230', 0, 2),
(4, 'Av. del Viento 101, Juana Koslay', 'Comercial', 3, 1, 180000.000000000000000000000000000000, '-33.2843,-66.2571', 1, 2),
(5, 'Calle 25, Manzana 10, La Punta', 'Residencial', 4, 0, 90000.000000000000000000000000000000, '-33.1784,-66.3155', 1, 3),
(6, 'Las Tipas 234, Potrero de los Funes', 'Residencial', 1, 5, 450000.000000000000000000000000000000, '-33.2188,-66.2290', 0, 3);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `Dni` longtext NOT NULL,
  `Nombre` longtext NOT NULL,
  `Apellido` longtext NOT NULL,
  `Email` longtext NOT NULL,
  `Telefono` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `Dni`, `Nombre`, `Apellido`, `Email`, `Telefono`) VALUES
(1, '35888999', 'Ana', 'Martinez', 'anam@mail.com', '2664101010'),
(2, '38777666', 'Javier', 'Fernandez', 'javierf@mail.com', '2664202020'),
(3, '40111222', 'Sofia', 'Lopez', 'sofial@mail.com', '2657606060');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `NumeroPago` int(11) NOT NULL,
  `ContratoId` int(11) NOT NULL,
  `FechaPago` datetime(6) NOT NULL,
  `Importe` decimal(10,2) NOT NULL,
  `Detalle` longtext NOT NULL,
  `Estado` longtext NOT NULL,
  `UsuarioIdAnulador` int(11) DEFAULT NULL,
  `UsuarioIdCreador` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `NumeroPago`, `ContratoId`, `FechaPago`, `Importe`, `Detalle`, `Estado`, `UsuarioIdAnulador`, `UsuarioIdCreador`) VALUES
(1, 1, 1, '2023-03-05 00:00:00.000000', 250000.00, 'Pago mes de Marzo', 'Vigente', NULL, 4),
(2, 2, 1, '2023-04-04 00:00:00.000000', 250000.00, 'Pago mes de Abril', 'Vigente', NULL, 4),
(3, 1, 3, '2024-01-15 00:00:00.000000', 450000.00, 'Pago mes de Enero', 'Vigente', NULL, 3),
(4, 2, 3, '2024-02-14 00:00:00.000000', 450000.00, 'Pago duplicado por error', 'Anulado', 3, 4),
(5, 3, 3, '2024-02-15 00:00:00.000000', 450000.00, 'Pago mes de Febrero (correcto)', 'Vigente', NULL, 3);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `Dni` longtext NOT NULL,
  `Nombre` longtext NOT NULL,
  `Apellido` longtext NOT NULL,
  `Email` longtext NOT NULL,
  `Telefono` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `Dni`, `Nombre`, `Apellido`, `Email`, `Telefono`) VALUES
(1, '25111222', 'Carlos', 'Perez', 'carlosp@mail.com', '2664858585'),
(2, '30333444', 'Laura', 'Gomez', 'laurag@mail.com', '2664747474'),
(3, '28555666', 'Miguel', 'Rodriguez', 'miguelr@mail.com', '2657303030');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tiposinmuebles`
--

CREATE TABLE `tiposinmuebles` (
  `Id` int(11) NOT NULL,
  `Nombre` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tiposinmuebles`
--

INSERT INTO `tiposinmuebles` (`Id`, `Nombre`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(3, 'Local Comercial'),
(4, 'Lote'),
(5, 'Cabaña');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id` int(11) NOT NULL,
  `Nombre` longtext NOT NULL,
  `Apellido` longtext NOT NULL,
  `Email` longtext NOT NULL,
  `Clave` longtext NOT NULL,
  `Rol` longtext NOT NULL,
  `Avatar` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id`, `Nombre`, `Apellido`, `Email`, `Clave`, `Rol`, `Avatar`) VALUES
(1, 'Admin', 'User', 'admin@inmobiliaria.com', '$2a$11$6A.rxHF1AwxlWan3udYLbOkfqtKxTg5yNjKLzh0SkaRH0nG7Rc5sa', 'Administrador', '/uploads/avatars/3902a8f7-80a9-413f-884f-69ceda011aab_A determined individual.jpeg'),
(2, 'Empleado', 'Delmes', 'empleadodelmes@inmobiliaria.com', 'contraseña', 'Empleado', NULL),
(5, 'kevin', 'test', 'Kevinsito1559@gmail.com', '$2a$11$ELzs6eCqxwgPGeje6CVcdeWOa27xKvFZpq/SrrgGMLnCRYNUfNvvS', 'Empleado', NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250917210533_InitialCreate', '9.0.9'),
('20250918033542_PropietarioMigration', '9.0.9'),
('20250918034909_InquilinoMigration', '9.0.9'),
('20250918035748_TipoInmuebleMigration', '9.0.9'),
('20250918041754_InmuebleMigration', '9.0.9'),
('20250921223913_ContratoMigration', '9.0.9'),
('20250921231441_PagoMigration', '9.0.9'),
('20250921234532_ContratoTerminacionMigration', '9.0.9'),
('20251006200624_AuditoriaMigration', '9.0.9');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Contratos_InmuebleId` (`InmuebleId`),
  ADD KEY `IX_Contratos_InquilinoId` (`InquilinoId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Inmuebles_PropietarioId` (`PropietarioId`),
  ADD KEY `IX_Inmuebles_TipoInmuebleId` (`TipoInmuebleId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Pagos_ContratoId` (`ContratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `tiposinmuebles`
--
ALTER TABLE `tiposinmuebles`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `tiposinmuebles`
--
ALTER TABLE `tiposinmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `FK_Contratos_Inmuebles_InmuebleId` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Contratos_Inquilinos_InquilinoId` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `FK_Inmuebles_Propietarios_PropietarioId` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Inmuebles_TiposInmuebles_TipoInmuebleId` FOREIGN KEY (`TipoInmuebleId`) REFERENCES `tiposinmuebles` (`Id`) ON DELETE CASCADE;

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `FK_Pagos_Contratos_ContratoId` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
