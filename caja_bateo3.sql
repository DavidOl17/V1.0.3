-- phpMyAdmin SQL Dump
-- version 4.7.4
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 11-01-2018 a las 13:13:27
-- Versión del servidor: 10.1.29-MariaDB
-- Versión de PHP: 7.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `caja_bateo`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `creditos_aderidos`
--

CREATE TABLE `creditos_aderidos` (
  `id_tarjeta` int(11) NOT NULL,
  `fecha_adicion` varchar(25) NOT NULL,
  `fecha_vencimiento` date NOT NULL,
  `creditos_aderidos` int(11) NOT NULL,
  `creditos_disponibles` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `creditos_aderidos`
--

INSERT INTO `creditos_aderidos` (`id_tarjeta`, `fecha_adicion`, `fecha_vencimiento`, `creditos_aderidos`, `creditos_disponibles`) VALUES
(10021, '09/01/2018 16:54:48', '2018-02-09', 11, 11),
(10021, '09/01/2018 16:55:46', '2018-02-09', 13, 13),
(10022, '09/01/2018 16:55:55', '2018-02-09', 15, 15),
(10023, '09/01/2018 16:57:02', '2018-02-09', 10, 10),
(10024, '09/01/2018 16:57:10', '2018-02-09', 11, 11);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `creditos_mensuales`
--

CREATE TABLE `creditos_mensuales` (
  `id_tarjeta` int(11) NOT NULL,
  `fecha_adicion` varchar(15) NOT NULL,
  `fecha_vencimiento` date NOT NULL,
  `creditos_aderidos` int(11) NOT NULL,
  `creditos_disponibles` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `creditos_mensuales`
--

INSERT INTO `creditos_mensuales` (`id_tarjeta`, `fecha_adicion`, `fecha_vencimiento`, `creditos_aderidos`, `creditos_disponibles`) VALUES
(10015, '10/02/2018', '2018-02-10', 10, 10),
(10016, '10/02/2018', '2018-02-10', 10, 10),
(10017, '10/02/2018', '2018-02-10', 10, 10),
(10018, '10/02/2018', '2018-02-10', 10, 10),
(10019, '10/02/2018', '2018-02-10', 10, 10),
(10020, '10/02/2018', '2018-02-10', 10, 10),
(10021, '05/01/2018', '2018-02-05', 10, 0),
(10021, '07/05/2017', '2018-01-31', 15, 10),
(10021, '16/08/2017', '2017-09-16', 15, 0),
(10021, '20/02/2018', '2018-02-10', 10, 10),
(10022, '10/02/2018', '2018-02-10', 10, 10),
(10023, '10/02/2018', '2018-02-10', 10, 10),
(10024, '10/02/2018', '2018-02-10', 10, 10);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tarjeta`
--

CREATE TABLE `tarjeta` (
  `id_tarjeta` int(11) NOT NULL,
  `fecha_creacion` varchar(20) NOT NULL,
  `status` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Volcado de datos para la tabla `tarjeta`
--

INSERT INTO `tarjeta` (`id_tarjeta`, `fecha_creacion`, `status`) VALUES
(10000, '09/01/2018', 1),
(10001, '09/01/2018', 1),
(10002, '09/01/2018', 1),
(10003, '09/01/2018', 1),
(10004, '09/01/2018', 1),
(10005, '09/01/2018', 1),
(10006, '09/01/2018', 1),
(10007, '09/01/2018', 1),
(10008, '09/01/2018', 1),
(10009, '09/01/2018', 1),
(10010, '09/01/2018', 1),
(10011, '09/01/2018', 1),
(10012, '09/01/2018', 1),
(10013, '09/01/2018', 1),
(10014, '09/01/2018', 1),
(10015, '09/01/2018', 1),
(10016, '09/01/2018', 1),
(10017, '09/01/2018', 1),
(10018, '09/01/2018', 1),
(10019, '09/01/2018', 1),
(10020, '09/01/2018', 1),
(10021, '09/01/2018', 1),
(10022, '09/01/2018', 1),
(10023, '09/01/2018', 1),
(10024, '09/01/2018', 1),
(10025, '09/01/2018', 1),
(10026, '09/01/2018', 1),
(10027, '09/01/2018', 1),
(10028, '09/01/2018', 1),
(10029, '09/01/2018', 1),
(10030, '09/01/2018', 1),
(10031, '09/01/2018', 1),
(10032, '09/01/2018', 1),
(10033, '09/01/2018', 1),
(10034, '09/01/2018', 1),
(10035, '09/01/2018', 1),
(10036, '09/01/2018', 1),
(10037, '09/01/2018', 1),
(10038, '09/01/2018', 1),
(10039, '09/01/2018', 1),
(10040, '09/01/2018', 1),
(10041, '09/01/2018', 1),
(10042, '09/01/2018', 1),
(10043, '09/01/2018', 1),
(10044, '09/01/2018', 1),
(10045, '09/01/2018', 1),
(10046, '09/01/2018', 1),
(10047, '09/01/2018', 1),
(10048, '09/01/2018', 1),
(10049, '09/01/2018', 1),
(10050, '09/01/2018', 1),
(10051, '09/01/2018', 1),
(10052, '09/01/2018', 1),
(10053, '09/01/2018', 1),
(10054, '09/01/2018', 1),
(10055, '09/01/2018', 1),
(10056, '09/01/2018', 1),
(10057, '09/01/2018', 1),
(10058, '09/01/2018', 1),
(10059, '09/01/2018', 1),
(10060, '09/01/2018', 1),
(10061, '09/01/2018', 1),
(10062, '09/01/2018', 1),
(10063, '09/01/2018', 1),
(10064, '09/01/2018', 1),
(10065, '09/01/2018', 1),
(10066, '09/01/2018', 1),
(10067, '09/01/2018', 1),
(10068, '09/01/2018', 1),
(10069, '09/01/2018', 1),
(10070, '09/01/2018', 1),
(10071, '09/01/2018', 1),
(10072, '09/01/2018', 1),
(10073, '09/01/2018', 1),
(10074, '09/01/2018', 1),
(10075, '09/01/2018', 1),
(10076, '09/01/2018', 1),
(10077, '09/01/2018', 1),
(10078, '09/01/2018', 1),
(10079, '09/01/2018', 1),
(10080, '09/01/2018', 1),
(10081, '09/01/2018', 1),
(10082, '09/01/2018', 1),
(10083, '09/01/2018', 1),
(10084, '09/01/2018', 1),
(10085, '09/01/2018', 1),
(10086, '09/01/2018', 1),
(10087, '09/01/2018', 1),
(10088, '09/01/2018', 1),
(10089, '09/01/2018', 1),
(10090, '09/01/2018', 1),
(10091, '09/01/2018', 1),
(10092, '09/01/2018', 1),
(10093, '09/01/2018', 1),
(10094, '09/01/2018', 1),
(10095, '09/01/2018', 1),
(10096, '09/01/2018', 1),
(10097, '09/01/2018', 1),
(10098, '09/01/2018', 1),
(10099, '09/01/2018', 1),
(10100, '09/01/2018', 1),
(10101, '09/01/2018', 1),
(10102, '09/01/2018', 1),
(10103, '09/01/2018', 1),
(10104, '09/01/2018', 1),
(10105, '09/01/2018', 1),
(10106, '09/01/2018', 1),
(10107, '09/01/2018', 1),
(10108, '09/01/2018', 1),
(10109, '09/01/2018', 1),
(10110, '09/01/2018', 1),
(10111, '09/01/2018', 1),
(10112, '09/01/2018', 1),
(10113, '09/01/2018', 1),
(10114, '09/01/2018', 1),
(10115, '09/01/2018', 1),
(10116, '10/01/2018', 1),
(10117, '09/01/2018', 1),
(10118, '09/01/2018', 1),
(10119, '09/01/2018', 1),
(10120, '09/01/2018', 1),
(10121, '09/01/2018', 1),
(10122, '09/01/2018', 1),
(10123, '09/01/2018', 1),
(10124, '09/01/2018', 0);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `creditos_aderidos`
--
ALTER TABLE `creditos_aderidos`
  ADD PRIMARY KEY (`id_tarjeta`,`fecha_adicion`);

--
-- Indices de la tabla `creditos_mensuales`
--
ALTER TABLE `creditos_mensuales`
  ADD PRIMARY KEY (`id_tarjeta`,`fecha_adicion`);

--
-- Indices de la tabla `tarjeta`
--
ALTER TABLE `tarjeta`
  ADD PRIMARY KEY (`id_tarjeta`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `tarjeta`
--
ALTER TABLE `tarjeta`
  MODIFY `id_tarjeta` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10125;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `creditos_aderidos`
--
ALTER TABLE `creditos_aderidos`
  ADD CONSTRAINT `tarjeta_adicionales_res` FOREIGN KEY (`id_tarjeta`) REFERENCES `tarjeta` (`id_tarjeta`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `creditos_mensuales`
--
ALTER TABLE `creditos_mensuales`
  ADD CONSTRAINT `tarjeta_mensual_res` FOREIGN KEY (`id_tarjeta`) REFERENCES `tarjeta` (`id_tarjeta`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
