CREATE DATABASE  IF NOT EXISTS `ferremas_integrada` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `ferremas_integrada`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: ferremas_integrada
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ajustes_inventario`
--

DROP TABLE IF EXISTS `ajustes_inventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ajustes_inventario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL,
  `usuario_id` int NOT NULL,
  `motivo` enum('conteo_fisico','merma','sobrante','otros') NOT NULL,
  `observaciones` text,
  `estado` enum('pendiente','aprobado','rechazado') NOT NULL DEFAULT 'pendiente',
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `ajustes_inventario_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ajustes_inventario`
--

LOCK TABLES `ajustes_inventario` WRITE;
/*!40000 ALTER TABLE `ajustes_inventario` DISABLE KEYS */;
/*!40000 ALTER TABLE `ajustes_inventario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `carrito`
--

DROP TABLE IF EXISTS `carrito`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `carrito` (
  `id` int NOT NULL AUTO_INCREMENT,
  `cliente_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad` int NOT NULL,
  `precio_unitario` decimal(10,2) NOT NULL,
  `fecha_agregado` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `cliente_id` (`cliente_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `carrito_ibfk_1` FOREIGN KEY (`cliente_id`) REFERENCES `clientes` (`id`) ON DELETE CASCADE,
  CONSTRAINT `carrito_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `carrito`
--

LOCK TABLES `carrito` WRITE;
/*!40000 ALTER TABLE `carrito` DISABLE KEYS */;
INSERT INTO `carrito` VALUES (1,1,3,1,12990.00,'2025-05-13 00:02:35'),(2,1,5,2,1990.00,'2025-05-13 00:02:35');
/*!40000 ALTER TABLE `carrito` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categorias`
--

DROP TABLE IF EXISTS `categorias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categorias` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` varchar(500) DEFAULT NULL,
  `categoria_padre_id` int DEFAULT NULL,
  `codigo` varchar(10) NOT NULL,
  `activo` tinyint(1) DEFAULT '1',
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categorias`
--

LOCK TABLES `categorias` WRITE;
/*!40000 ALTER TABLE `categorias` DISABLE KEYS */;
INSERT INTO `categorias` VALUES (1,'Herramientas Eléctricas',NULL,NULL,'HE',1,'2025-05-26 04:19:49',NULL),(2,'Herramientas Manuales',NULL,NULL,'HM',1,'2025-05-26 04:19:49',NULL),(3,'Jardinería',NULL,NULL,'JA',1,'2025-05-26 04:19:49',NULL),(4,'Construcción',NULL,NULL,'CO',1,'2025-05-26 04:19:49',NULL),(5,'Electricidad',NULL,NULL,'EL',1,'2025-05-26 04:19:49',NULL),(6,'Plomería',NULL,NULL,'PL',1,'2025-05-26 04:19:49',NULL),(7,'Pintura y Acabados',NULL,NULL,'PA',1,'2025-05-26 04:19:49',NULL),(8,'Seguridad Industrial',NULL,NULL,'SI',1,'2025-05-26 04:19:49',NULL);
/*!40000 ALTER TABLE `categorias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clientes`
--

DROP TABLE IF EXISTS `clientes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clientes` (
  `id` int NOT NULL AUTO_INCREMENT,
  `telefono` varchar(20) DEFAULT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `rut` varchar(20) DEFAULT NULL,
  `correo_electronico` varchar(100) DEFAULT NULL,
  `fecha_creacion` datetime DEFAULT NULL,
  `fecha_modificacion` datetime DEFAULT NULL,
  `activo` tinyint(1) DEFAULT NULL,
  `usuario_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_clientes_usuario_id` (`usuario_id`),
  CONSTRAINT `fk_clientes_usuarios` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clientes`
--

LOCK TABLES `clientes` WRITE;
/*!40000 ALTER TABLE `clientes` DISABLE KEYS */;
INSERT INTO `clientes` VALUES (1,'+56 9 1234 5678',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL),(2,'+56934567890','Javier','Ramírez','56789012-3','javier.ramirez@example.com','2025-05-25 17:09:07','2025-05-25 17:46:47',0,NULL),(3,'+56987654321','Valentina','Muñoz','34567890-1','valentina.munoz2@example.com','2025-05-25 17:31:26','2025-05-25 17:47:02',0,NULL),(4,'+56923456789','Camila','Pérez','45678901-2','camila.perez@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL),(5,'+56911223344','Valentina','Lagos','98765432-1','valentina.lagos@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL),(6,'+56999887766','Diego','Rojas','19283746-5','diego.rojas@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL),(7,'+56955667788','Fernanda','Salazar','56473829-0','fernanda.salazar@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL),(8,'+56933445566','Tomás','González','37482910-4','tomas.gonzalez@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL),(9,'+56966778899','Sofía','Fuentes','10293847-3','sofia.fuentes@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL),(10,'+56944556677','Cristóbal','Navarro','56473820-6','cristobal.navarro@example.com','2025-05-25 17:42:13','2025-05-25 17:42:13',1,NULL);
/*!40000 ALTER TABLE `clientes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clientes_suscripcion`
--

DROP TABLE IF EXISTS `clientes_suscripcion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clientes_suscripcion` (
  `id` int NOT NULL AUTO_INCREMENT,
  `cliente_id` int NOT NULL,
  `tipo_suscripcion` enum('newsletter','ofertas','nuevos_productos','todas') NOT NULL,
  `fecha_suscripcion` datetime DEFAULT CURRENT_TIMESTAMP,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `cliente_id` (`cliente_id`),
  CONSTRAINT `clientes_suscripcion_ibfk_1` FOREIGN KEY (`cliente_id`) REFERENCES `clientes` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clientes_suscripcion`
--

LOCK TABLES `clientes_suscripcion` WRITE;
/*!40000 ALTER TABLE `clientes_suscripcion` DISABLE KEYS */;
INSERT INTO `clientes_suscripcion` VALUES (1,1,'todas','2025-05-13 00:02:36',1);
/*!40000 ALTER TABLE `clientes_suscripcion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comisiones`
--

DROP TABLE IF EXISTS `comisiones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comisiones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `empleado_id` int NOT NULL,
  `venta_id` int NOT NULL,
  `porcentaje` decimal(5,2) NOT NULL,
  `monto` decimal(18,2) NOT NULL,
  `fecha` datetime NOT NULL,
  `estado` enum('pendiente','pagada') NOT NULL DEFAULT 'pendiente',
  PRIMARY KEY (`id`),
  KEY `empleado_id` (`empleado_id`),
  KEY `venta_id` (`venta_id`),
  CONSTRAINT `comisiones_ibfk_1` FOREIGN KEY (`empleado_id`) REFERENCES `empleados` (`id`),
  CONSTRAINT `comisiones_ibfk_2` FOREIGN KEY (`venta_id`) REFERENCES `pedidos` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comisiones`
--

LOCK TABLES `comisiones` WRITE;
/*!40000 ALTER TABLE `comisiones` DISABLE KEYS */;
/*!40000 ALTER TABLE `comisiones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comparaciones_historial`
--

DROP TABLE IF EXISTS `comparaciones_historial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comparaciones_historial` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int NOT NULL,
  `nombre_producto` varchar(200) NOT NULL,
  `precio_ferremas` decimal(18,2) NOT NULL,
  `resultado_json` text NOT NULL,
  `fecha` datetime NOT NULL,
  `FechaComparacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `comparaciones_historial_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comparaciones_historial`
--

LOCK TABLES `comparaciones_historial` WRITE;
/*!40000 ALTER TABLE `comparaciones_historial` DISABLE KEYS */;
INSERT INTO `comparaciones_historial` VALUES (1,13,'taladro inalámbrico',50000.00,'[]','2025-05-27 00:08:01',NULL),(2,13,'taladro inalámbrico',50000.00,'[]','2025-05-27 00:14:07',NULL),(3,13,'taladro inalámbrico',50000.00,'[]','2025-05-27 00:16:15',NULL),(4,13,'taladro inalámbrico',50000.00,'[]','2025-05-27 00:20:11',NULL),(5,13,'taladro inalámbrico',50000.00,'[]','2025-05-27 00:24:02',NULL),(6,13,'Taladro Percutor 650W',59990.00,'[{\"Tienda\":\"Mercado Libre\",\"NombreProducto\":\"Taladro Percutor 650W Bosch Professional\",\"Precio\":10173,\"Enlace\":\"https://mercadolibre.cl/demo\",\"Imagen\":\"https://via.placeholder.com/150x150?text=Demo\",\"Vendedor\":\"Vendedor 42\",\"ReputacionVendedor\":3,\"DiferenciaPrecio\":49817,\"PorcentajeDiferencia\":83.04217369561593598933155526,\"ReputacionVendedorFormatted\":\"3.0\",\"CantidadVentas\":0,\"Condicion\":\"\",\"EnvioGratis\":false,\"Ubicacion\":\"\",\"IdProducto\":\"\",\"EstadoCompetitividad\":\"\",\"CantidadDisponible\":0,\"FechaConsulta\":\"2025-05-28T19:44:02.3206369Z\"},{\"Tienda\":\"Mercado Libre\",\"NombreProducto\":\"Taladro Percutor 650W Stanley FatMax\",\"Precio\":17656,\"Enlace\":\"https://mercadolibre.cl/demo\",\"Imagen\":\"https://via.placeholder.com/150x150?text=Demo\",\"Vendedor\":\"Vendedor 50\",\"ReputacionVendedor\":1,\"DiferenciaPrecio\":42334,\"PorcentajeDiferencia\":70.568428071345224204034005670,\"ReputacionVendedorFormatted\":\"1.0\",\"CantidadVentas\":0,\"Condicion\":\"\",\"EnvioGratis\":false,\"Ubicacion\":\"\",\"IdProducto\":\"\",\"EstadoCompetitividad\":\"\",\"CantidadDisponible\":0,\"FechaConsulta\":\"2025-05-28T19:44:02.3211064Z\"},{\"Tienda\":\"Mercado Libre\",\"NombreProducto\":\"Taladro Percutor 650W DeWalt 20V Max\",\"Precio\":19453,\"Enlace\":\"https://mercadolibre.cl/demo\",\"Imagen\":\"https://via.placeholder.com/150x150?text=Demo\",\"Vendedor\":\"Vendedor 51\",\"ReputacionVendedor\":2,\"DiferenciaPrecio\":40537,\"PorcentajeDiferencia\":67.572928821470245040840140020,\"ReputacionVendedorFormatted\":\"2.0\",\"CantidadVentas\":0,\"Condicion\":\"\",\"EnvioGratis\":false,\"Ubicacion\":\"\",\"IdProducto\":\"\",\"EstadoCompetitividad\":\"\",\"CantidadDisponible\":0,\"FechaConsulta\":\"2025-05-28T19:44:02.3211078Z\"},{\"Tienda\":\"Mercado Libre\",\"NombreProducto\":\"Taladro Percutor 650W Makita 18V\",\"Precio\":21922,\"Enlace\":\"https://mercadolibre.cl/demo\",\"Imagen\":\"https://via.placeholder.com/150x150?text=Demo\",\"Vendedor\":\"Vendedor 88\",\"ReputacionVendedor\":5,\"DiferenciaPrecio\":38068,\"PorcentajeDiferencia\":63.457242873812302050341723620,\"ReputacionVendedorFormatted\":\"5.0\",\"CantidadVentas\":0,\"Condicion\":\"\",\"EnvioGratis\":false,\"Ubicacion\":\"\",\"IdProducto\":\"\",\"EstadoCompetitividad\":\"\",\"CantidadDisponible\":0,\"FechaConsulta\":\"2025-05-28T19:44:02.3211082Z\"},{\"Tienda\":\"Mercado Libre\",\"NombreProducto\":\"Taladro Percutor 650W Black \\u0026 Decker\",\"Precio\":22008,\"Enlace\":\"https://mercadolibre.cl/demo\",\"Imagen\":\"https://via.placeholder.com/150x150?text=Demo\",\"Vendedor\":\"Vendedor 76\",\"ReputacionVendedor\":1,\"DiferenciaPrecio\":37982,\"PorcentajeDiferencia\":63.313885647607934655775962660,\"ReputacionVendedorFormatted\":\"1.0\",\"CantidadVentas\":0,\"Condicion\":\"\",\"EnvioGratis\":false,\"Ubicacion\":\"\",\"IdProducto\":\"\",\"EstadoCompetitividad\":\"\",\"CantidadDisponible\":0,\"FechaConsulta\":\"2025-05-28T19:44:02.3211087Z\"}]','2025-05-28 19:44:02',NULL);
/*!40000 ALTER TABLE `comparaciones_historial` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comparacioneshistorial`
--

DROP TABLE IF EXISTS `comparacioneshistorial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comparacioneshistorial` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UsuarioEmail` varchar(255) NOT NULL,
  `Producto` varchar(255) NOT NULL,
  `PrecioFerremas` decimal(18,2) NOT NULL,
  `Fecha` datetime NOT NULL,
  `ResultadoJson` text NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comparacioneshistorial`
--

LOCK TABLES `comparacioneshistorial` WRITE;
/*!40000 ALTER TABLE `comparacioneshistorial` DISABLE KEYS */;
/*!40000 ALTER TABLE `comparacioneshistorial` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comparadorprecios`
--

DROP TABLE IF EXISTS `comparadorprecios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comparadorprecios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ProductoId` int NOT NULL,
  `TipoFuente` enum('sucursal','competidor') NOT NULL,
  `FuenteId` int NOT NULL,
  `Competidor` varchar(100) DEFAULT NULL,
  `PrecioCompetidor` decimal(10,2) DEFAULT NULL,
  `FechaConsulta` datetime DEFAULT CURRENT_TIMESTAMP,
  `UrlProducto` varchar(500) DEFAULT NULL,
  `TiendaId` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `producto_id` (`ProductoId`),
  CONSTRAINT `comparadorprecios_ibfk_1` FOREIGN KEY (`ProductoId`) REFERENCES `productos` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comparadorprecios`
--

LOCK TABLES `comparadorprecios` WRITE;
/*!40000 ALTER TABLE `comparadorprecios` DISABLE KEYS */;
INSERT INTO `comparadorprecios` VALUES (1,1,'sucursal',0,'Sodimac',62000.00,'2025-05-13 00:02:35',NULL,NULL),(2,1,'sucursal',0,'Imperial',63500.00,'2025-05-13 00:02:35',NULL,NULL),(4,2,'sucursal',0,'Imperial',10500.00,'2025-05-13 00:02:35',NULL,NULL),(5,3,'sucursal',0,'Sodimac',15590.00,'2025-05-26 20:09:51',NULL,NULL),(6,3,'sucursal',0,'Imperial',12500.00,'2025-05-13 00:02:35',NULL,NULL),(7,4,'sucursal',0,'Sodimac',6200.00,'2025-05-13 00:02:35',NULL,NULL),(9,5,'sucursal',0,'Sodimac',2100.00,'2025-05-13 00:02:35',NULL,NULL),(11,6,'sucursal',0,'Sodimac',15500.00,'2025-05-13 00:02:35',NULL,NULL),(12,6,'sucursal',0,'Imperial',14800.00,'2025-05-13 00:02:35',NULL,NULL),(13,7,'sucursal',0,'Sodimac',4200.00,'2025-05-13 00:02:35',NULL,NULL),(14,7,'sucursal',0,'Imperial',3850.00,'2025-05-13 00:02:35',NULL,NULL),(15,8,'sucursal',0,'Sodimac',8100.00,'2025-05-13 00:02:35',NULL,NULL),(16,8,'sucursal',0,'Imperial',7850.00,'2025-05-13 00:02:35',NULL,NULL),(17,1,'sucursal',1,NULL,19990.00,'2025-05-24 22:14:14',NULL,NULL),(18,1,'competidor',2,'Sodimac',21990.00,'2025-05-24 22:14:19',NULL,NULL),(19,1,'sucursal',1,NULL,19990.00,'2025-05-24 22:17:12',NULL,NULL),(20,1,'sucursal',2,NULL,20990.00,'2025-05-24 22:17:12',NULL,NULL),(21,1,'competidor',1,'Sodimac',21990.00,'2025-05-24 22:17:23',NULL,NULL),(22,1,'competidor',2,'Imperial',18990.00,'2025-05-24 22:17:23',NULL,NULL),(23,1,'competidor',3,'Easy',19990.00,'2025-05-24 22:17:23',NULL,NULL),(24,1,'competidor',0,'Sodimac',62000.00,'2025-05-13 00:02:35',NULL,NULL),(25,1,'competidor',0,'Imperial',63500.00,'2025-05-13 00:02:35',NULL,NULL),(26,2,'competidor',0,'Sodimac',10200.00,'2025-05-13 00:02:35',NULL,NULL),(27,2,'competidor',0,'Imperial',10500.00,'2025-05-13 00:02:35',NULL,NULL),(28,3,'competidor',0,'Sodimac',13500.00,'2025-05-13 00:02:35',NULL,NULL),(29,3,'competidor',0,'Imperial',12500.00,'2025-05-13 00:02:35',NULL,NULL),(30,4,'competidor',0,'Sodimac',6200.00,'2025-05-13 00:02:35',NULL,NULL),(31,4,'competidor',0,'Imperial',6150.00,'2025-05-13 00:02:35',NULL,NULL),(32,5,'competidor',0,'Sodimac',2100.00,'2025-05-13 00:02:35',NULL,NULL),(33,5,'competidor',0,'Imperial',1950.00,'2025-05-13 00:02:35',NULL,NULL),(34,6,'competidor',0,'Sodimac',15500.00,'2025-05-13 00:02:35',NULL,NULL),(35,6,'competidor',0,'Imperial',14800.00,'2025-05-13 00:02:35',NULL,NULL),(36,7,'competidor',0,'Sodimac',4200.00,'2025-05-13 00:02:35',NULL,NULL),(37,7,'competidor',0,'Imperial',3850.00,'2025-05-13 00:02:35',NULL,NULL),(38,8,'competidor',0,'Sodimac',8100.00,'2025-05-13 00:02:35',NULL,NULL),(39,8,'competidor',0,'Imperial',7850.00,'2025-05-13 00:02:35',NULL,NULL),(55,1,'competidor',1,'Sodimac',19990.00,'2025-05-26 20:09:07','https://www.sodimac.cl/sodimac-cl/product/123456',NULL);
/*!40000 ALTER TABLE `comparadorprecios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `competidores`
--

DROP TABLE IF EXISTS `competidores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `competidores` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `url` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `competidores`
--

LOCK TABLES `competidores` WRITE;
/*!40000 ALTER TABLE `competidores` DISABLE KEYS */;
INSERT INTO `competidores` VALUES (1,'Sodimac','https://www.sodimac.cl'),(2,'Imperial','https://www.imperial.cl'),(3,'Easy','https://www.easy.cl');
/*!40000 ALTER TABLE `competidores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `configuracion_sistema`
--

DROP TABLE IF EXISTS `configuracion_sistema`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuracion_sistema` (
  `id` int NOT NULL AUTO_INCREMENT,
  `clave` varchar(50) NOT NULL,
  `valor` text,
  `descripcion` text,
  `tipo_valor` enum('texto','numero','booleano','json') NOT NULL,
  `modificable` tinyint(1) DEFAULT '1',
  `fecha_modificacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `clave` (`clave`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `configuracion_sistema`
--

LOCK TABLES `configuracion_sistema` WRITE;
/*!40000 ALTER TABLE `configuracion_sistema` DISABLE KEYS */;
INSERT INTO `configuracion_sistema` VALUES (1,'envio_gratis_min','50000','Monto mínimo para envío gratis','numero',1,'2025-05-13 04:02:36'),(2,'descuento_4_articulos','10','Porcentaje de descuento al comprar 4 o más artículos','numero',1,'2025-05-13 04:02:36'),(3,'tiempo_sesion','120','Tiempo de sesión en minutos','numero',1,'2025-05-13 04:02:36');
/*!40000 ALTER TABLE `configuracion_sistema` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `conversion_divisas`
--

DROP TABLE IF EXISTS `conversion_divisas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `conversion_divisas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `moneda_origen` varchar(10) NOT NULL,
  `moneda_destino` varchar(10) NOT NULL,
  `tasa` decimal(10,4) NOT NULL,
  `fecha` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `conversion_divisas`
--

LOCK TABLES `conversion_divisas` WRITE;
/*!40000 ALTER TABLE `conversion_divisas` DISABLE KEYS */;
INSERT INTO `conversion_divisas` VALUES (1,'CLP','USD',0.0010,'2025-05-13 00:02:35'),(2,'CLP','EUR',0.0010,'2025-05-13 00:02:35'),(3,'USD','CLP',970.5000,'2025-05-13 00:02:35'),(4,'EUR','CLP',1045.7500,'2025-05-13 00:02:35');
/*!40000 ALTER TABLE `conversion_divisas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `conversiondivisas`
--

DROP TABLE IF EXISTS `conversiondivisas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `conversiondivisas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `MonedaOrigen` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MonedaDestino` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TasaCambio` decimal(65,30) NOT NULL,
  `FechaActualizacion` datetime(6) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `conversiondivisas`
--

LOCK TABLES `conversiondivisas` WRITE;
/*!40000 ALTER TABLE `conversiondivisas` DISABLE KEYS */;
/*!40000 ALTER TABLE `conversiondivisas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalles_ajuste_inventario`
--

DROP TABLE IF EXISTS `detalles_ajuste_inventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalles_ajuste_inventario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ajuste_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad_sistema` int NOT NULL,
  `cantidad_fisica` int NOT NULL,
  `diferencia` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ajuste_id` (`ajuste_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `detalles_ajuste_inventario_ibfk_1` FOREIGN KEY (`ajuste_id`) REFERENCES `ajustes_inventario` (`id`),
  CONSTRAINT `detalles_ajuste_inventario_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalles_ajuste_inventario`
--

LOCK TABLES `detalles_ajuste_inventario` WRITE;
/*!40000 ALTER TABLE `detalles_ajuste_inventario` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalles_ajuste_inventario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalles_orden_compra`
--

DROP TABLE IF EXISTS `detalles_orden_compra`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalles_orden_compra` (
  `id` int NOT NULL AUTO_INCREMENT,
  `orden_compra_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad` int NOT NULL,
  `precio_unitario` decimal(18,2) NOT NULL,
  `subtotal` decimal(18,2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `orden_compra_id` (`orden_compra_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `detalles_orden_compra_ibfk_1` FOREIGN KEY (`orden_compra_id`) REFERENCES `ordenes_compra` (`id`),
  CONSTRAINT `detalles_orden_compra_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalles_orden_compra`
--

LOCK TABLES `detalles_orden_compra` WRITE;
/*!40000 ALTER TABLE `detalles_orden_compra` DISABLE KEYS */;
/*!40000 ALTER TABLE `detalles_orden_compra` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detallespedido`
--

DROP TABLE IF EXISTS `detallespedido`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detallespedido` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pedido_id` int DEFAULT NULL,
  `producto_id` int DEFAULT NULL,
  `cantidad` int DEFAULT NULL,
  `precio_unitario` decimal(10,2) DEFAULT NULL,
  `subtotal` decimal(10,2) DEFAULT NULL,
  `observaciones` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pedido_id` (`pedido_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `detallespedido_ibfk_1` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`),
  CONSTRAINT `detallespedido_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`),
  CONSTRAINT `fk_detallespedido_pedido` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_detallespedido_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detallespedido`
--

LOCK TABLES `detallespedido` WRITE;
/*!40000 ALTER TABLE `detallespedido` DISABLE KEYS */;
INSERT INTO `detallespedido` VALUES (1,1,1,1,59990.00,NULL,NULL),(2,1,2,1,9990.00,NULL,NULL),(3,1,7,1,3990.00,NULL,NULL),(4,1,4,2,5990.00,11980.00,'Entrega rápida'),(5,1,4,1,1000.00,1000.00,NULL),(6,1,10,2,1500.00,3000.00,NULL),(7,1,10,2,1500.00,3000.00,NULL),(8,1,11,1,2000.00,2000.00,NULL),(9,1,12,3,500.00,1500.00,NULL),(10,3,1,2,5000.00,10000.00,NULL),(11,4,1,2,5000.00,10000.00,NULL);
/*!40000 ALTER TABLE `detallespedido` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `devoluciones`
--

DROP TABLE IF EXISTS `devoluciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `devoluciones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pedido_id` int NOT NULL,
  `usuario_id` int NOT NULL,
  `motivo` text NOT NULL,
  `estado` enum('solicitada','en_revision','aprobada','rechazada','completada') DEFAULT 'solicitada',
  `monto_devolucion` decimal(10,2) DEFAULT NULL,
  `fecha_solicitud` datetime DEFAULT CURRENT_TIMESTAMP,
  `fecha_resolucion` datetime DEFAULT NULL,
  `comentarios_internos` text,
  PRIMARY KEY (`id`),
  KEY `pedido_id` (`pedido_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `devoluciones_ibfk_1` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`),
  CONSTRAINT `devoluciones_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `devoluciones`
--

LOCK TABLES `devoluciones` WRITE;
/*!40000 ALTER TABLE `devoluciones` DISABLE KEYS */;
/*!40000 ALTER TABLE `devoluciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `direcciones`
--

DROP TABLE IF EXISTS `direcciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `direcciones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int DEFAULT NULL,
  `calle` varchar(150) NOT NULL,
  `numero` varchar(20) NOT NULL,
  `departamento` varchar(50) DEFAULT NULL,
  `comuna` varchar(100) NOT NULL,
  `region` varchar(100) NOT NULL,
  `codigo_postal` varchar(20) DEFAULT NULL,
  `es_principal` tinyint(1) DEFAULT NULL,
  `FechaCreacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `FechaModificacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `direcciones_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `direcciones`
--

LOCK TABLES `direcciones` WRITE;
/*!40000 ALTER TABLE `direcciones` DISABLE KEYS */;
INSERT INTO `direcciones` VALUES (1,1,'Av. Los Presidentes','123',NULL,'Santiago','Región Metropolitana',NULL,0,'2025-05-23 01:38:20',NULL),(2,2,'Calle Los Olmos','456',NULL,'Puerto Montt','Los Lagos',NULL,0,'2025-05-23 01:38:20',NULL),(3,3,'Av. Los Leones','123',NULL,'Santiago','Región Metropolitana',NULL,0,'2025-05-23 01:38:20',NULL),(4,3,'Calle Providencia','789',NULL,'Providencia','Región Metropolitana',NULL,0,'2025-05-23 01:38:20',NULL),(5,4,'Pasaje Las Dalias','101',NULL,'Ñuñoa','Región Metropolitana',NULL,0,'2025-05-23 01:38:20',NULL),(6,2,'Avenida Las Flores','456','12A','Las Condes','Metropolitana','7550000',1,'2025-05-25 17:09:07','2025-05-25 17:04:09'),(7,3,'Avenida Las Flores','456','12A','Las Condes','Metropolitana','7550000',1,'2025-05-25 17:31:26','2025-05-25 17:26:41'),(8,4,'Avenida Central','321','8C','Ñuñoa','Metropolitana','7770000',1,'2025-05-25 17:42:13','2025-05-25 18:00:00');
/*!40000 ALTER TABLE `direcciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `direcciones_backup`
--

DROP TABLE IF EXISTS `direcciones_backup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `direcciones_backup` (
  `id` int NOT NULL DEFAULT '0',
  `usuario_id` int NOT NULL,
  `calle` varchar(150) NOT NULL,
  `numero` varchar(20) NOT NULL,
  `departamento` varchar(50) DEFAULT NULL,
  `comuna` varchar(100) NOT NULL,
  `region` varchar(100) NOT NULL,
  `CodigoPostal` varchar(20) DEFAULT NULL,
  `EsPrincipal` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `direcciones_backup`
--

LOCK TABLES `direcciones_backup` WRITE;
/*!40000 ALTER TABLE `direcciones_backup` DISABLE KEYS */;
INSERT INTO `direcciones_backup` VALUES (1,1,'Av. Los Presidentes','123',NULL,'Santiago','Región Metropolitana',NULL,0),(2,2,'Calle Los Olmos','456',NULL,'Puerto Montt','Los Lagos',NULL,0),(3,3,'Av. Los Leones','123',NULL,'Santiago','Región Metropolitana',NULL,0),(4,3,'Calle Providencia','789',NULL,'Providencia','Región Metropolitana',NULL,0),(5,4,'Pasaje Las Dalias','101',NULL,'Ñuñoa','Región Metropolitana',NULL,0);
/*!40000 ALTER TABLE `direcciones_backup` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleados`
--

DROP TABLE IF EXISTS `empleados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleados` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `rut` varchar(20) NOT NULL,
  `fecha_contratacion` date NOT NULL,
  `cargo` varchar(100) NOT NULL,
  `departamento` varchar(100) NOT NULL,
  `sucursal_id` int NOT NULL,
  `activo` tinyint(1) DEFAULT '1',
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `rut` (`rut`),
  KEY `usuario_id` (`usuario_id`),
  KEY `sucursal_id` (`sucursal_id`),
  CONSTRAINT `empleados_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `empleados_ibfk_2` FOREIGN KEY (`sucursal_id`) REFERENCES `sucursales` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleados`
--

LOCK TABLES `empleados` WRITE;
/*!40000 ALTER TABLE `empleados` DISABLE KEYS */;
/*!40000 ALTER TABLE `empleados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `envios`
--

DROP TABLE IF EXISTS `envios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `envios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pedido_id` int DEFAULT NULL,
  `repartidor_id` int DEFAULT NULL,
  `fecha_envio` datetime DEFAULT CURRENT_TIMESTAMP,
  `estado_envio` varchar(20) DEFAULT NULL,
  `direccion_envio` varchar(200) NOT NULL,
  `proveedor_transporte` varchar(100) NOT NULL,
  `tracking_url` varchar(200) NOT NULL,
  `observaciones` varchar(500) DEFAULT NULL,
  `comuna_destino` varchar(50) NOT NULL,
  `region_destino` varchar(50) NOT NULL,
  `telefono_contacto` varchar(20) NOT NULL,
  `nombre_destinatario` varchar(100) NOT NULL,
  `fecha_creacion` datetime NOT NULL,
  `fecha_actualizacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pedido_id` (`pedido_id`),
  KEY `repartidor_id` (`repartidor_id`),
  CONSTRAINT `envios_ibfk_1` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`),
  CONSTRAINT `envios_ibfk_2` FOREIGN KEY (`repartidor_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `envios`
--

LOCK TABLES `envios` WRITE;
/*!40000 ALTER TABLE `envios` DISABLE KEYS */;
INSERT INTO `envios` VALUES (1,1,4,'2025-05-13 00:00:00','2','','Shipit','',NULL,'','','','','2025-05-26 20:50:55',NULL),(5,1,4,'2025-05-26 20:51:05','EN_PREPARACION','Av. Los Presidentes 123','Shipit','SHIPIT-123456','Sin observaciones','Santiago','Región Metropolitana','+56912345678','Juan Pérez','2025-05-26 20:51:05',NULL),(6,1,4,'2025-05-26 20:51:05','DESPACHADO','Calle Los Olmos 456','Chilexpress','CHILEX-789012','Entregar en portería','Puerto Montt','Los Lagos','+56987654321','María González','2025-05-26 20:51:05',NULL),(7,1,4,'2025-05-26 21:03:02','EN_PREPARACION','Av. Los Presidentes 123, Depto 45','Shipit','SHIPIT-123456','Sin observaciones','Santiago','Región Metropolitana','+56912345678','Juan Pérez','2025-05-26 21:03:02',NULL),(8,2,NULL,'0001-01-01 00:00:00','Pendiente','Av. Los Presidentes 123, Depto 45','Shipit','','Envío a Santiago, Región Metropolitana','Santiago','Región Metropolitana','+56912345678','Juan Pérez','2025-05-28 19:45:45',NULL);
/*!40000 ALTER TABLE `envios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `facturas`
--

DROP TABLE IF EXISTS `facturas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facturas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pedido_id` int NOT NULL,
  `numero_factura` varchar(20) NOT NULL,
  `fecha_emision` datetime NOT NULL,
  `subtotal` decimal(10,2) NOT NULL,
  `iva` decimal(10,2) NOT NULL,
  `total` decimal(10,2) NOT NULL,
  `url_pdf` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `pedido_id` (`pedido_id`),
  UNIQUE KEY `numero_factura` (`numero_factura`),
  CONSTRAINT `facturas_ibfk_1` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `facturas`
--

LOCK TABLES `facturas` WRITE;
/*!40000 ALTER TABLE `facturas` DISABLE KEYS */;
INSERT INTO `facturas` VALUES (1,1,'F-001','2025-05-12 00:00:00',72244.00,13726.00,85970.00,'/facturas/F-001.pdf');
/*!40000 ALTER TABLE `facturas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `garantias`
--

DROP TABLE IF EXISTS `garantias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `garantias` (
  `id` int NOT NULL AUTO_INCREMENT,
  `venta_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cliente_id` int NOT NULL,
  `numero_garantia` varchar(20) NOT NULL,
  `fecha_inicio` datetime NOT NULL,
  `fecha_fin` datetime NOT NULL,
  `estado` enum('activa','vencida','utilizada') NOT NULL DEFAULT 'activa',
  `tipo_garantia` enum('fabricante','tienda') NOT NULL,
  `observaciones` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `numero_garantia` (`numero_garantia`),
  KEY `venta_id` (`venta_id`),
  KEY `producto_id` (`producto_id`),
  KEY `cliente_id` (`cliente_id`),
  CONSTRAINT `garantias_ibfk_1` FOREIGN KEY (`venta_id`) REFERENCES `pedidos` (`id`),
  CONSTRAINT `garantias_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`),
  CONSTRAINT `garantias_ibfk_3` FOREIGN KEY (`cliente_id`) REFERENCES `clientes` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `garantias`
--

LOCK TABLES `garantias` WRITE;
/*!40000 ALTER TABLE `garantias` DISABLE KEYS */;
/*!40000 ALTER TABLE `garantias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historial_navegacion`
--

DROP TABLE IF EXISTS `historial_navegacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historial_navegacion` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int DEFAULT NULL,
  `producto_id` int DEFAULT NULL,
  `fecha_vista` datetime DEFAULT CURRENT_TIMESTAMP,
  `duracion_segundos` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `historial_navegacion_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `historial_navegacion_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historial_navegacion`
--

LOCK TABLES `historial_navegacion` WRITE;
/*!40000 ALTER TABLE `historial_navegacion` DISABLE KEYS */;
INSERT INTO `historial_navegacion` VALUES (1,3,1,'2025-05-13 00:02:35',45),(2,3,2,'2025-05-13 00:02:35',30),(3,3,3,'2025-05-13 00:02:35',20),(4,3,4,'2025-05-13 00:02:35',15),(5,3,5,'2025-05-13 00:02:35',25),(6,3,6,'2025-05-13 00:02:35',40),(7,3,7,'2025-05-13 00:02:35',10),(8,3,8,'2025-05-13 00:02:35',35);
/*!40000 ALTER TABLE `historial_navegacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historial_precios`
--

DROP TABLE IF EXISTS `historial_precios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historial_precios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `fecha_cambio` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `usuario_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `producto_id` (`producto_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `historial_precios_ibfk_1` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`),
  CONSTRAINT `historial_precios_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historial_precios`
--

LOCK TABLES `historial_precios` WRITE;
/*!40000 ALTER TABLE `historial_precios` DISABLE KEYS */;
INSERT INTO `historial_precios` VALUES (1,1,57990.00,'2025-05-13 00:02:35',1),(2,1,59990.00,'2025-05-13 00:02:35',1),(3,2,8990.00,'2025-05-13 00:02:35',1),(4,2,9990.00,'2025-05-13 00:02:35',1),(5,3,11990.00,'2025-05-13 00:02:35',1),(6,3,12990.00,'2025-05-13 00:02:35',1),(7,4,5790.00,'2025-05-13 00:02:35',1),(8,4,5990.00,'2025-05-13 00:02:35',1);
/*!40000 ALTER TABLE `historial_precios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `horarios`
--

DROP TABLE IF EXISTS `horarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `horarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `empleado_id` int NOT NULL,
  `dia_semana` enum('lunes','martes','miercoles','jueves','viernes','sabado','domingo') NOT NULL,
  `hora_inicio` time NOT NULL,
  `hora_fin` time NOT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `empleado_id` (`empleado_id`),
  CONSTRAINT `horarios_ibfk_1` FOREIGN KEY (`empleado_id`) REFERENCES `empleados` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `horarios`
--

LOCK TABLES `horarios` WRITE;
/*!40000 ALTER TABLE `horarios` DISABLE KEYS */;
/*!40000 ALTER TABLE `horarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `integracion_banco_central`
--

DROP TABLE IF EXISTS `integracion_banco_central`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `integracion_banco_central` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha_consulta` datetime DEFAULT CURRENT_TIMESTAMP,
  `url_api` varchar(255) NOT NULL,
  `parametros_consulta` text,
  `resultado` json DEFAULT NULL,
  `estado` enum('exitoso','error') NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `integracion_banco_central`
--

LOCK TABLES `integracion_banco_central` WRITE;
/*!40000 ALTER TABLE `integracion_banco_central` DISABLE KEYS */;
INSERT INTO `integracion_banco_central` VALUES (1,'2025-05-13 00:02:36','https://api.banco-central.cl/api/divisas/dolar','fecha=2025-05-12','{\"fecha\": \"2025-05-12\", \"valor\": 970.5}','exitoso'),(2,'2025-05-13 00:02:36','https://api.banco-central.cl/api/divisas/euro','fecha=2025-05-12','{\"fecha\": \"2025-05-12\", \"valor\": 1045.75}','exitoso');
/*!40000 ALTER TABLE `integracion_banco_central` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `integracion_webpay`
--

DROP TABLE IF EXISTS `integracion_webpay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `integracion_webpay` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pago_id` int NOT NULL,
  `token_ws` varchar(255) NOT NULL,
  `url_retorno` varchar(255) NOT NULL,
  `fecha_creacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_expiracion` datetime NOT NULL,
  `respuesta_completa` json DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pago_id` (`pago_id`),
  CONSTRAINT `integracion_webpay_ibfk_1` FOREIGN KEY (`pago_id`) REFERENCES `pagos` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `integracion_webpay`
--

LOCK TABLES `integracion_webpay` WRITE;
/*!40000 ALTER TABLE `integracion_webpay` DISABLE KEYS */;
/*!40000 ALTER TABLE `integracion_webpay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventario`
--

DROP TABLE IF EXISTS `inventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `sucursal_id` int DEFAULT NULL,
  `stock_actual` int NOT NULL DEFAULT '0',
  `stock_minimo` int NOT NULL DEFAULT '5',
  `ultima_actualizacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `producto_id` (`producto_id`),
  KEY `sucursal_id` (`sucursal_id`),
  CONSTRAINT `inventario_ibfk_1` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`),
  CONSTRAINT `inventario_ibfk_2` FOREIGN KEY (`sucursal_id`) REFERENCES `sucursales` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventario`
--

LOCK TABLES `inventario` WRITE;
/*!40000 ALTER TABLE `inventario` DISABLE KEYS */;
INSERT INTO `inventario` VALUES (1,1,1,15,5,'2025-05-13 04:02:35'),(2,1,2,10,5,'2025-05-13 04:02:35'),(3,2,1,30,5,'2025-05-13 04:02:35'),(4,2,2,20,5,'2025-05-13 04:02:35'),(5,3,1,20,5,'2025-05-13 04:02:35'),(6,3,2,10,5,'2025-05-13 04:02:35'),(7,4,1,50,5,'2025-05-13 04:02:35'),(8,4,2,50,5,'2025-05-13 04:02:35'),(9,5,1,100,5,'2025-05-13 04:02:35'),(10,5,2,100,5,'2025-05-13 04:02:35'),(11,6,1,25,5,'2025-05-13 04:02:35'),(12,6,2,15,5,'2025-05-13 04:02:35'),(13,7,1,40,5,'2025-05-13 04:02:35'),(14,7,2,20,5,'2025-05-13 04:02:35'),(15,8,1,20,5,'2025-05-13 04:02:35'),(16,8,2,15,5,'2025-05-13 04:02:35');
/*!40000 ALTER TABLE `inventario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `listas_deseos`
--

DROP TABLE IF EXISTS `listas_deseos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `listas_deseos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `fecha_agregado` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_wishlist_item` (`usuario_id`,`producto_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `listas_deseos_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `listas_deseos_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `listas_deseos`
--

LOCK TABLES `listas_deseos` WRITE;
/*!40000 ALTER TABLE `listas_deseos` DISABLE KEYS */;
INSERT INTO `listas_deseos` VALUES (1,3,4,'2025-05-13 00:02:36'),(2,3,6,'2025-05-13 00:02:36');
/*!40000 ALTER TABLE `listas_deseos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `logs`
--

DROP TABLE IF EXISTS `logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `logs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int DEFAULT NULL,
  `accion` varchar(255) NOT NULL,
  `tabla_afectada` varchar(50) DEFAULT NULL,
  `registro_id` int DEFAULT NULL,
  `datos_previos` text,
  `datos_nuevos` text,
  `ip` varchar(45) DEFAULT NULL,
  `fecha` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `logs_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `logs`
--

LOCK TABLES `logs` WRITE;
/*!40000 ALTER TABLE `logs` DISABLE KEYS */;
INSERT INTO `logs` VALUES (1,1,'CREACIÓN','productos',1,NULL,NULL,NULL,'2025-05-13 04:02:36'),(2,1,'ACTUALIZACIÓN','productos',1,NULL,NULL,NULL,'2025-05-13 04:02:36'),(3,2,'CREACIÓN','pedidos',1,NULL,NULL,NULL,'2025-05-13 04:02:36'),(4,1,'ACTUALIZACIÓN','inventario',1,NULL,NULL,NULL,'2025-05-13 04:02:36'),(5,1,'CREACIÓN','promociones',1,NULL,NULL,NULL,'2025-05-13 04:02:36');
/*!40000 ALTER TABLE `logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `marcas`
--

DROP TABLE IF EXISTS `marcas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `marcas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` text,
  `logo_url` varchar(255) DEFAULT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `nombre` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marcas`
--

LOCK TABLES `marcas` WRITE;
/*!40000 ALTER TABLE `marcas` DISABLE KEYS */;
INSERT INTO `marcas` VALUES (1,'Bosch','Herramientas eléctricas alemanas',NULL,1),(2,'Makita','Marca japonesa de herramientas',NULL,1),(3,'Stanley','Herramientas manuales de calidad',NULL,1),(4,'Sika','Productos químicos para construcción',NULL,1),(5,'Philips','Productos eléctricos y electrónicos',NULL,1),(6,'3M','Productos industriales y de seguridad',NULL,1);
/*!40000 ALTER TABLE `marcas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimientos_inventario`
--

DROP TABLE IF EXISTS `movimientos_inventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimientos_inventario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `ubicacion_id` int NOT NULL,
  `tipo_movimiento` enum('entrada','salida','ajuste') NOT NULL,
  `cantidad` int NOT NULL,
  `fecha` datetime NOT NULL,
  `usuario_id` int NOT NULL,
  `referencia` varchar(100) DEFAULT NULL,
  `observaciones` text,
  PRIMARY KEY (`id`),
  KEY `producto_id` (`producto_id`),
  KEY `ubicacion_id` (`ubicacion_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `movimientos_inventario_ibfk_1` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`),
  CONSTRAINT `movimientos_inventario_ibfk_2` FOREIGN KEY (`ubicacion_id`) REFERENCES `ubicaciones_almacen` (`id`),
  CONSTRAINT `movimientos_inventario_ibfk_3` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimientos_inventario`
--

LOCK TABLES `movimientos_inventario` WRITE;
/*!40000 ALTER TABLE `movimientos_inventario` DISABLE KEYS */;
/*!40000 ALTER TABLE `movimientos_inventario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notificaciones`
--

DROP TABLE IF EXISTS `notificaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notificaciones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int NOT NULL,
  `titulo` varchar(100) NOT NULL,
  `mensaje` text NOT NULL,
  `leida` tinyint(1) DEFAULT '0',
  `fecha_creacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_lectura` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `notificaciones_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notificaciones`
--

LOCK TABLES `notificaciones` WRITE;
/*!40000 ALTER TABLE `notificaciones` DISABLE KEYS */;
INSERT INTO `notificaciones` VALUES (1,1,'Stock Bajo','El producto \"Llave Stilson 14\" tiene stock bajo (5 unidades)',0,'2025-05-13 04:02:36',NULL),(2,3,'Pedido Enviado','Su pedido #1 ha sido enviado y será entregado pronto',0,'2025-05-13 04:02:36',NULL),(3,2,'Nueva Valoración','Un cliente ha dejado una nueva valoración para el producto \"Taladro Percutor 650W\"',0,'2025-05-13 04:02:36',NULL),(4,4,'Pedido para Entregar','Tienes un nuevo pedido asignado para entregar',0,'2025-05-13 04:02:36',NULL);
/*!40000 ALTER TABLE `notificaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ordenes_compra`
--

DROP TABLE IF EXISTS `ordenes_compra`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordenes_compra` (
  `id` int NOT NULL AUTO_INCREMENT,
  `proveedor_id` int NOT NULL,
  `numero_orden` varchar(20) NOT NULL,
  `fecha_orden` datetime NOT NULL,
  `fecha_entrega_esperada` datetime DEFAULT NULL,
  `estado` enum('pendiente','aprobada','recibida','cancelada') NOT NULL DEFAULT 'pendiente',
  `total` decimal(18,2) NOT NULL,
  `observaciones` text,
  `usuario_id` int NOT NULL,
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `numero_orden` (`numero_orden`),
  KEY `proveedor_id` (`proveedor_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `ordenes_compra_ibfk_1` FOREIGN KEY (`proveedor_id`) REFERENCES `proveedores` (`id`),
  CONSTRAINT `ordenes_compra_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ordenes_compra`
--

LOCK TABLES `ordenes_compra` WRITE;
/*!40000 ALTER TABLE `ordenes_compra` DISABLE KEYS */;
/*!40000 ALTER TABLE `ordenes_compra` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pagos`
--

DROP TABLE IF EXISTS `pagos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `pedido_id` int NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `fecha_pago` datetime DEFAULT CURRENT_TIMESTAMP,
  `estado` varchar(20) DEFAULT NULL,
  `metodo_pago` enum('MERCADOPAGO') NOT NULL,
  `transaccion_id` varchar(100) DEFAULT NULL,
  `token_pasarela` varchar(100) DEFAULT NULL,
  `datos_respuesta` text,
  PRIMARY KEY (`id`),
  KEY `pedido_id` (`pedido_id`),
  CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pagos`
--

LOCK TABLES `pagos` WRITE;
/*!40000 ALTER TABLE `pagos` DISABLE KEYS */;
INSERT INTO `pagos` VALUES (5,1,49990.00,'2024-05-28 10:15:00','aprobado','MERCADOPAGO','MP-123456','tok_test_1','{\"status\":\"approved\",\"payment_method\":\"visa\"}'),(6,2,15990.00,'2024-05-28 11:20:00','aprobado','MERCADOPAGO','MP-123457','tok_test_2','{\"status\":\"approved\",\"payment_method\":\"mastercard\"}'),(7,3,8990.00,'2024-05-28 12:05:00','pendiente','MERCADOPAGO','MP-123458','tok_test_3','{\"status\":\"pending\",\"payment_method\":\"visa\"}'),(8,4,25990.00,'2024-05-28 13:30:00','rechazado','MERCADOPAGO','MP-123459','tok_test_4','{\"status\":\"rejected\",\"payment_method\":\"visa\"}'),(9,5,19990.00,'2024-05-28 14:10:00','aprobado','MERCADOPAGO','MP-123460','tok_test_5','{\"status\":\"approved\",\"payment_method\":\"amex\"}'),(10,6,29990.00,'2024-05-28 15:45:00','aprobado','MERCADOPAGO','MP-123461','tok_test_6','{\"status\":\"approved\",\"payment_method\":\"visa\"}'),(11,7,34990.00,'2024-05-28 16:00:00','pendiente','MERCADOPAGO','MP-123462','tok_test_7','{\"status\":\"pending\",\"payment_method\":\"mastercard\"}'),(12,8,12990.00,'2024-05-28 16:30:00','aprobado','MERCADOPAGO','MP-123463','tok_test_8','{\"status\":\"approved\",\"payment_method\":\"visa\"}'),(13,9,17990.00,'2024-05-28 17:10:00','rechazado','MERCADOPAGO','MP-123464','tok_test_9','{\"status\":\"rejected\",\"payment_method\":\"amex\"}'),(14,10,21990.00,'2024-05-28 18:00:00','aprobado','MERCADOPAGO','MP-123465','tok_test_10','{\"status\":\"approved\",\"payment_method\":\"visa\"}');
/*!40000 ALTER TABLE `pagos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pedidos`
--

DROP TABLE IF EXISTS `pedidos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedidos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int DEFAULT NULL,
  `fecha_pedido` datetime DEFAULT NULL,
  `estado` varchar(20) DEFAULT NULL,
  `fecha_creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` datetime DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `repartidor_id` int DEFAULT NULL,
  `total` decimal(10,2) DEFAULT NULL,
  `observaciones` varchar(500) DEFAULT NULL,
  `direccion_entrega` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `pedidos_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `clientes` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pedidos`
--

LOCK TABLES `pedidos` WRITE;
/*!40000 ALTER TABLE `pedidos` DISABLE KEYS */;
INSERT INTO `pedidos` VALUES (1,1,'2025-05-12 00:00:00','Enviado','2025-05-26 11:24:22',NULL,1,NULL,85970.00,NULL,NULL),(2,1,'2025-05-28 09:03:59','Pendiente','2025-05-28 09:03:59',NULL,1,NULL,5000.00,NULL,NULL),(3,1,'2025-05-28 13:07:17','Pendiente','2025-05-28 13:07:17',NULL,1,NULL,10000.00,'Pedido de prueba','Calle Falsa 123'),(4,1,'2025-05-28 13:11:10','Pendiente','2025-05-28 13:11:10',NULL,1,NULL,10000.00,'Pedido de prueba','Calle Falsa 123'),(5,1,'2024-05-28 08:00:00','aprobado','2025-05-28 18:28:13',NULL,1,NULL,49990.00,'Entrega rápida','Av. Los Presidentes 123, Depto 45'),(6,1,'2024-05-28 08:10:00','aprobado','2025-05-28 18:28:13',NULL,1,NULL,15990.00,NULL,'Calle Falsa 123'),(7,1,'2024-05-28 08:20:00','pendiente','2025-05-28 18:28:13',NULL,1,NULL,8990.00,NULL,'Av. Providencia 456'),(8,1,'2024-05-28 08:30:00','rechazado','2025-05-28 18:28:13',NULL,1,NULL,25990.00,'Cliente no contestó','Las Condes 789'),(9,1,'2024-05-28 08:40:00','aprobado','2025-05-28 18:28:13',NULL,1,NULL,19990.00,NULL,'Ñuñoa 321'),(10,1,'2024-05-28 08:50:00','aprobado','2025-05-28 18:28:13',NULL,1,NULL,29990.00,NULL,'Maipú 654'),(11,1,'2024-05-28 09:00:00','pendiente','2025-05-28 18:28:13',NULL,1,NULL,34990.00,NULL,'Puente Alto 987'),(12,1,'2024-05-28 09:10:00','aprobado','2025-05-28 18:28:13',NULL,1,NULL,12990.00,NULL,'La Florida 111'),(13,1,'2024-05-28 09:20:00','rechazado','2025-05-28 18:28:13',NULL,1,NULL,17990.00,NULL,'San Bernardo 222'),(14,1,'2024-05-28 09:30:00','aprobado','2025-05-28 18:28:13',NULL,1,NULL,21990.00,NULL,'Temuco 333'),(15,1,'2024-05-28 08:00:00','aprobado','2025-05-28 18:29:46',NULL,1,NULL,49990.00,'Entrega rápida','Av. Los Presidentes 123, Depto 45'),(16,1,'2024-05-28 08:10:00','aprobado','2025-05-28 18:29:46',NULL,1,NULL,15990.00,NULL,'Calle Falsa 123'),(17,1,'2024-05-28 08:20:00','pendiente','2025-05-28 18:29:46',NULL,1,NULL,8990.00,NULL,'Av. Providencia 456'),(18,1,'2024-05-28 08:30:00','rechazado','2025-05-28 18:29:46',NULL,1,NULL,25990.00,'Cliente no contestó','Las Condes 789'),(19,1,'2024-05-28 08:40:00','aprobado','2025-05-28 18:29:46',NULL,1,NULL,19990.00,NULL,'Ñuñoa 321'),(20,1,'2024-05-28 08:50:00','aprobado','2025-05-28 18:29:46',NULL,1,NULL,29990.00,NULL,'Maipú 654'),(21,1,'2024-05-28 09:00:00','pendiente','2025-05-28 18:29:46',NULL,1,NULL,34990.00,NULL,'Puente Alto 987'),(22,1,'2024-05-28 09:10:00','aprobado','2025-05-28 18:29:46',NULL,1,NULL,12990.00,NULL,'La Florida 111'),(23,1,'2024-05-28 09:20:00','rechazado','2025-05-28 18:29:46',NULL,1,NULL,17990.00,NULL,'San Bernardo 222'),(24,1,'2024-05-28 09:30:00','aprobado','2025-05-28 18:29:46',NULL,1,NULL,21990.00,NULL,'Temuco 333');
/*!40000 ALTER TABLE `pedidos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productos`
--

DROP TABLE IF EXISTS `productos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) NOT NULL,
  `codigo` varchar(50) NOT NULL,
  `descripcion` text,
  `precio` decimal(10,2) DEFAULT NULL,
  `stock` int DEFAULT NULL,
  `categoria_id` int DEFAULT NULL,
  `marca_id` int DEFAULT NULL,
  `ImagenUrl` varchar(200) NOT NULL DEFAULT 'Sin imagen',
  `especificaciones` varchar(1000) DEFAULT NULL,
  `fecha_creacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` datetime DEFAULT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `stock_minimo` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `categoria_id` (`categoria_id`),
  KEY `marca_id` (`marca_id`),
  CONSTRAINT `productos_ibfk_1` FOREIGN KEY (`categoria_id`) REFERENCES `categorias` (`id`),
  CONSTRAINT `productos_ibfk_2` FOREIGN KEY (`marca_id`) REFERENCES `marcas` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productos`
--

LOCK TABLES `productos` WRITE;
/*!40000 ALTER TABLE `productos` DISABLE KEYS */;
INSERT INTO `productos` VALUES (1,'Taladro Percutor 650W','HE100','Taladro eléctrico de 650W',59990.00,21,1,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46','2025-05-28 13:11:10',1,1),(2,'Martillo de Carpintero','HM101','Martillo de acero de 16 oz',9990.00,50,2,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46',NULL,1,1),(3,'Tijera de Podar','JA102','Tijera para poda de jardín',12990.00,30,3,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46',NULL,1,1),(4,'Cemento Portland 25kg','CO103','Bolsa de cemento de 25kg',5990.00,100,4,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46','2025-05-26 14:44:13',1,1),(5,'Cable Eléctrico 2x1.5mm','EL104','Cable para instalaciones eléctricas',1990.00,200,5,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46',NULL,1,1),(6,'Llave Stilson 14\"','PL105','Llave ajustable para plomería',14990.00,40,6,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46',NULL,1,1),(7,'Rodillo para Pintura 9\"','PA106','Rodillo de espuma para pintura',3990.00,60,7,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46',NULL,1,1),(8,'Casco de Seguridad Amarillo','SI107','Casco de protección industrial',7990.00,35,8,NULL,'Sin imagen',NULL,'2025-05-26 03:35:46',NULL,1,1),(10,'Destornillador','P002','Destornillador de cruz',1990.00,50,1,2,'https://ejemplo.com/destornillador.jpg','Material: acero','2025-05-26 03:37:16',NULL,1,1),(11,'Alicate','P003','Alicate universal',2990.00,30,2,1,'Sin imagen','Longitud: 15cm','2025-05-26 03:37:16',NULL,1,1),(12,'Taladro','P004','Taladro eléctrico 500W',24990.00,10,3,3,'https://ejemplo.com/taladro.jpg','Incluye brocas','2025-05-26 03:37:16',NULL,1,1),(13,'Llave Inglesa','P005','Llave ajustable',3990.00,25,2,2,'Sin imagen','Apertura máxima: 30mm','2025-05-26 03:37:16',NULL,1,1),(14,'Martillo','P001','Martillo de acero',4990.00,120,1,2,'https://ejemplo.com/martillo.jpg','Peso: 500g','2025-05-26 08:03:22','2025-05-26 16:47:54',1,1),(17,'Sierra Circular Profesional 2000W','HE108','Sierra circular profesional de alta potencia con sistema de seguridad integrado y guía láser',89990.00,15,1,1,'https://ejemplo.com/sierra-circular.jpg','Potencia: 2000W, Diámetro de disco: 184mm, Velocidad: 5500 RPM, Peso: 4.2kg','2025-05-28 05:57:01',NULL,1,1),(18,'Martillo','HE100','acero',2990.00,100,2,1,'string','string','2025-05-28 23:18:26',NULL,1,1);
/*!40000 ALTER TABLE `productos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productos_relacionados`
--

DROP TABLE IF EXISTS `productos_relacionados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productos_relacionados` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `producto_relacionado_id` int NOT NULL,
  `tipo_relacion` enum('accesorios','complementarios','alternativos','repuestos') NOT NULL,
  `orden` int DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_relacion` (`producto_id`,`producto_relacionado_id`),
  KEY `producto_relacionado_id` (`producto_relacionado_id`),
  CONSTRAINT `productos_relacionados_ibfk_1` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE CASCADE,
  CONSTRAINT `productos_relacionados_ibfk_2` FOREIGN KEY (`producto_relacionado_id`) REFERENCES `productos` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productos_relacionados`
--

LOCK TABLES `productos_relacionados` WRITE;
/*!40000 ALTER TABLE `productos_relacionados` DISABLE KEYS */;
INSERT INTO `productos_relacionados` VALUES (1,1,2,'complementarios',1),(2,1,5,'accesorios',2),(3,7,5,'complementarios',1);
/*!40000 ALTER TABLE `productos_relacionados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `promociones`
--

DROP TABLE IF EXISTS `promociones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `promociones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) DEFAULT NULL,
  `descripcion` text,
  `porcentaje_descuento` decimal(5,2) DEFAULT NULL,
  `fecha_inicio` date DEFAULT NULL,
  `fecha_fin` date DEFAULT NULL,
  `activa` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `promociones`
--

LOCK TABLES `promociones` WRITE;
/*!40000 ALTER TABLE `promociones` DISABLE KEYS */;
INSERT INTO `promociones` VALUES (1,'Descuento Primavera','10% en herramientas seleccionadas',10.00,'2025-09-01','2025-09-30',1),(2,'Black Friday','30% en toda la tienda',30.00,'2025-11-25','2025-11-30',1),(3,'Cyber Day','25% en herramientas eléctricas',25.00,'2025-06-01','2025-06-03',1),(4,'Descuento Especial','15% en productos de jardinería',15.00,'2025-08-01','2025-08-15',1);
/*!40000 ALTER TABLE `promociones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proveedores`
--

DROP TABLE IF EXISTS `proveedores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proveedores` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `rut` varchar(20) NOT NULL,
  `direccion` varchar(200) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `contacto` varchar(100) DEFAULT NULL,
  `telefono_contacto` varchar(20) DEFAULT NULL,
  `email_contacto` varchar(100) DEFAULT NULL,
  `activo` tinyint(1) DEFAULT '1',
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `rut` (`rut`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proveedores`
--

LOCK TABLES `proveedores` WRITE;
/*!40000 ALTER TABLE `proveedores` DISABLE KEYS */;
/*!40000 ALTER TABLE `proveedores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recepcion_mercancia`
--

DROP TABLE IF EXISTS `recepcion_mercancia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recepcion_mercancia` (
  `id` int NOT NULL AUTO_INCREMENT,
  `orden_compra_id` int NOT NULL,
  `numero_recepcion` varchar(20) NOT NULL,
  `fecha_recepcion` datetime NOT NULL,
  `estado` enum('parcial','completa') NOT NULL,
  `observaciones` text,
  `usuario_id` int NOT NULL,
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `numero_recepcion` (`numero_recepcion`),
  KEY `orden_compra_id` (`orden_compra_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `recepcion_mercancia_ibfk_1` FOREIGN KEY (`orden_compra_id`) REFERENCES `ordenes_compra` (`id`),
  CONSTRAINT `recepcion_mercancia_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recepcion_mercancia`
--

LOCK TABLES `recepcion_mercancia` WRITE;
/*!40000 ALTER TABLE `recepcion_mercancia` DISABLE KEYS */;
/*!40000 ALTER TABLE `recepcion_mercancia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reparaciones`
--

DROP TABLE IF EXISTS `reparaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reparaciones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `servicio_tecnico_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `descripcion_problema` text NOT NULL,
  `diagnostico` text,
  `solucion` text,
  `costo_repuestos` decimal(18,2) DEFAULT '0.00',
  `costo_mano_obra` decimal(18,2) DEFAULT '0.00',
  `total` decimal(18,2) DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `servicio_tecnico_id` (`servicio_tecnico_id`),
  KEY `producto_id` (`producto_id`),
  CONSTRAINT `reparaciones_ibfk_1` FOREIGN KEY (`servicio_tecnico_id`) REFERENCES `servicios_tecnicos` (`id`),
  CONSTRAINT `reparaciones_ibfk_2` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reparaciones`
--

LOCK TABLES `reparaciones` WRITE;
/*!40000 ALTER TABLE `reparaciones` DISABLE KEYS */;
/*!40000 ALTER TABLE `reparaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `descripcion` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `nombre` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Administrador','Gestión completa del sistema'),(2,'Vendedor','Gestión de productos y pedidos'),(3,'Cliente','Usuario final que realiza compras'),(4,'Repartidor','Encargado de entregas y logística');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `seguimiento_garantias`
--

DROP TABLE IF EXISTS `seguimiento_garantias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `seguimiento_garantias` (
  `id` int NOT NULL AUTO_INCREMENT,
  `garantia_id` int NOT NULL,
  `fecha` datetime NOT NULL,
  `estado` enum('solicitada','en_revision','aprobada','rechazada','completada') NOT NULL,
  `descripcion` text NOT NULL,
  `usuario_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `garantia_id` (`garantia_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `seguimiento_garantias_ibfk_1` FOREIGN KEY (`garantia_id`) REFERENCES `garantias` (`id`),
  CONSTRAINT `seguimiento_garantias_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `seguimiento_garantias`
--

LOCK TABLES `seguimiento_garantias` WRITE;
/*!40000 ALTER TABLE `seguimiento_garantias` DISABLE KEYS */;
/*!40000 ALTER TABLE `seguimiento_garantias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `servicios_tecnicos`
--

DROP TABLE IF EXISTS `servicios_tecnicos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `servicios_tecnicos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `cliente_id` int NOT NULL,
  `empleado_id` int NOT NULL,
  `numero_servicio` varchar(20) NOT NULL,
  `tipo_servicio` enum('reparacion','mantenimiento','instalacion','otro') NOT NULL,
  `descripcion` text NOT NULL,
  `fecha_solicitud` datetime NOT NULL,
  `fecha_programada` datetime NOT NULL,
  `fecha_realizacion` datetime DEFAULT NULL,
  `estado` enum('pendiente','en_proceso','completado','cancelado') NOT NULL DEFAULT 'pendiente',
  `costo` decimal(18,2) DEFAULT NULL,
  `observaciones` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `numero_servicio` (`numero_servicio`),
  KEY `cliente_id` (`cliente_id`),
  KEY `empleado_id` (`empleado_id`),
  CONSTRAINT `servicios_tecnicos_ibfk_1` FOREIGN KEY (`cliente_id`) REFERENCES `clientes` (`id`),
  CONSTRAINT `servicios_tecnicos_ibfk_2` FOREIGN KEY (`empleado_id`) REFERENCES `empleados` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `servicios_tecnicos`
--

LOCK TABLES `servicios_tecnicos` WRITE;
/*!40000 ALTER TABLE `servicios_tecnicos` DISABLE KEYS */;
/*!40000 ALTER TABLE `servicios_tecnicos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sucursales`
--

DROP TABLE IF EXISTS `sucursales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sucursales` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) DEFAULT NULL,
  `direccion` text,
  `region` varchar(100) DEFAULT NULL,
  `comuna` varchar(100) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `horario_atencion` text,
  `encargado_id` int DEFAULT NULL,
  `fecha_creacion` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `encargado_id` (`encargado_id`),
  CONSTRAINT `sucursales_ibfk_1` FOREIGN KEY (`encargado_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sucursales`
--

LOCK TABLES `sucursales` WRITE;
/*!40000 ALTER TABLE `sucursales` DISABLE KEYS */;
INSERT INTO `sucursales` VALUES (1,'Sucursal Central','Av. Principal 123','Región Metropolitana','Santiago',NULL,NULL,NULL,1,'2025-05-13 04:02:35',1),(2,'Sucursal Sur','Ruta 5 Sur km 1000','Los Lagos','Puerto Montt',NULL,NULL,NULL,2,'2025-05-13 04:02:35',1),(3,'Sucursal Norte','Av. Balmaceda 345','Antofagasta','Antofagasta',NULL,NULL,NULL,1,'2025-05-13 04:02:35',1),(4,'Sucursal Oriente','Av. Las Condes 567','Región Metropolitana','Las Condes',NULL,NULL,NULL,2,'2025-05-13 04:02:35',1);
/*!40000 ALTER TABLE `sucursales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tiendas`
--

DROP TABLE IF EXISTS `tiendas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tiendas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Url` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tiendas`
--

LOCK TABLES `tiendas` WRITE;
/*!40000 ALTER TABLE `tiendas` DISABLE KEYS */;
/*!40000 ALTER TABLE `tiendas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ubicaciones_almacen`
--

DROP TABLE IF EXISTS `ubicaciones_almacen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ubicaciones_almacen` (
  `id` int NOT NULL AUTO_INCREMENT,
  `sucursal_id` int NOT NULL,
  `codigo` varchar(20) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `tipo` enum('estante','rack','gondola','bodega') NOT NULL,
  `capacidad` int DEFAULT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `codigo` (`codigo`),
  KEY `sucursal_id` (`sucursal_id`),
  CONSTRAINT `ubicaciones_almacen_ibfk_1` FOREIGN KEY (`sucursal_id`) REFERENCES `sucursales` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ubicaciones_almacen`
--

LOCK TABLES `ubicaciones_almacen` WRITE;
/*!40000 ALTER TABLE `ubicaciones_almacen` DISABLE KEYS */;
/*!40000 ALTER TABLE `ubicaciones_almacen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rut` varchar(12) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `rol` varchar(30) NOT NULL,
  `activo` tinyint(1) NOT NULL DEFAULT '1',
  `fecha_registro` datetime DEFAULT NULL,
  `ultimo_acceso` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  UNIQUE KEY `rut` (`rut`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'Juan Pérez','Pérez','juan@ferremas.cl','hashed123','12.345.678-9',NULL,'administrador',1,'2025-05-23 01:54:39',NULL),(2,'María González',NULL,'maria@ferremas.cl','hashed123','98.765.432-1',NULL,'vendedor',1,NULL,NULL),(3,'Carlos López',NULL,'carlos@cliente.cl','hashed123','11.223.344-5',NULL,'bodeguero',1,NULL,NULL),(4,'Ana','Martínez','ana@reparto.cl','hashed123','22.334.455-6','987654321','contador',1,'2025-05-26 07:03:55','2025-05-26 07:03:55'),(5,'Camila','Rojas','camila.rojas@mail.com','$2a$11$MNp.VB8mbwl4InfCtjRVzulCZwo1uLfVt0e/hG373060eZxxgesK6','18.765.432-1','+56987654321','bodeguero',1,'2025-05-23 03:10:02',NULL),(9,'Javier','Pérez','javier.perez@ferremas.cl','$2a$11$PGAUnNputAKmko9NJeyCXOD8t3bkhIi22G5JlMK3/57qjFlJJOQK6','22.334.556-7','+56912345678','bodeguero',1,'2025-05-23 03:11:58',NULL),(12,'Sofía','Valdés','sofia.valdes@ferremas.cl','$2a$11$KpEOY61ObJjYwggNffvICO4Pfb7vWaMlAkFiJkBXrqw9GW2m/7GpW','10.234.567-8','+56991234567','bodeguero',1,'2025-05-23 12:19:14',NULL),(13,'Paloma','Tamayo','palomatamayo@ferremas.cl','$2a$11$gSuRmEZ6bDL8Tjf1dFWXa.BkH9FDPkUzsST.5OQCOVBeb5ppoBwj6','12345678-9','123456789','administrador',1,'2025-05-26 06:54:40','2025-05-26 06:54:40'),(14,'Diego','Ramírez','diego.ramirez@ferremas.cl','$2a$11$kx0eIaGO6nc4oz.YZxL1YOhA50/IP.Aw3r6mKcAeRFGLuPIetUoYO','15.678.234-5','+56987654321','vendedor',1,'2025-05-25 18:01:25',NULL),(15,'Valentina','Muñoz','valentina.munoz@ferremas.cl','$2a$11$3KlA3ikGxW5RVNC31MMWYu5zE6uTnxfv/3cRoyUzwvh4QlMDVbiUq','21.345.678-9','+56991234567','bodeguero',1,'2025-05-25 18:25:58',NULL),(16,'Bruno','Tamayo','brunotamayo@example.com','$2a$11$OWWrJ7dRgi9sT46ISzH/Jug5t6J8nnuwGnrSRBlcClPC3pR7Y3ReG','28446521-0','+56966744055','administrador',1,'2025-05-26 02:58:13',NULL);
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `valoraciones_productos`
--

DROP TABLE IF EXISTS `valoraciones_productos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `valoraciones_productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `usuario_id` int NOT NULL,
  `puntuacion` int NOT NULL,
  `comentario` text,
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `producto_id` (`producto_id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `valoraciones_productos_ibfk_1` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`),
  CONSTRAINT `valoraciones_productos_ibfk_2` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`),
  CONSTRAINT `valoraciones_productos_chk_1` CHECK ((`puntuacion` between 1 and 5))
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `valoraciones_productos`
--

LOCK TABLES `valoraciones_productos` WRITE;
/*!40000 ALTER TABLE `valoraciones_productos` DISABLE KEYS */;
INSERT INTO `valoraciones_productos` VALUES (1,1,3,5,'Excelente taladro, muy potente y fácil de usar','2025-05-13 00:02:35'),(2,2,3,4,'Buen martillo, relación calidad-precio adecuada','2025-05-13 00:02:35'),(3,3,3,5,'Muy buenas tijeras, cortan perfectamente','2025-05-13 00:02:35'),(4,4,3,4,'Buen cemento, fraguado rápido','2025-05-13 00:02:35'),(5,5,3,5,'Cable de excelente calidad','2025-05-13 00:02:35'),(6,6,3,4,'Muy buena llave, agarre firme','2025-05-13 00:02:35'),(7,7,3,3,'Rodillo normal, cumple su función','2025-05-13 00:02:35'),(8,8,3,5,'Excelente casco, muy cómodo y seguro','2025-05-13 00:02:35');
/*!40000 ALTER TABLE `valoraciones_productos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'ferremas_integrada'
--
/*!50003 DROP PROCEDURE IF EXISTS `sp_actualizar_categoria` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_actualizar_categoria`(
    IN p_id INT,
    IN p_nombre VARCHAR(100),
    IN p_codigo VARCHAR(10)
)
BEGIN
    UPDATE categorias 
    SET nombre = p_nombre, 
        codigo = p_codigo
    WHERE id = p_id;
    
    SELECT 'Categoría actualizada correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_actualizar_estado_pedido` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_actualizar_estado_pedido`(
    IN p_pedido_id INT,
    IN p_estado VARCHAR(20)
)
BEGIN
    UPDATE pedidos 
    SET estado = p_estado
    WHERE id = p_pedido_id;
    
    SELECT 'Estado del pedido actualizado correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_actualizar_marca` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_actualizar_marca`(
    IN p_id INT,
    IN p_nombre VARCHAR(100),
    IN p_descripcion TEXT
)
BEGIN
    UPDATE marcas 
    SET nombre = p_nombre, 
        descripcion = p_descripcion
    WHERE id = p_id;
    
    SELECT 'Marca actualizada correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_actualizar_stock` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_actualizar_stock`(
    IN p_producto_id INT,
    IN p_sucursal_id INT,
    IN p_cantidad INT
)
BEGIN
    DECLARE stock_anterior INT;
    
    -- Obtener stock actual
    SELECT stock_actual INTO stock_anterior
    FROM inventario
    WHERE producto_id = p_producto_id AND sucursal_id = p_sucursal_id;
    
    -- Actualizar inventario
    UPDATE inventario
    SET stock_actual = p_cantidad,
        ultima_actualizacion = NOW()
    WHERE producto_id = p_producto_id AND sucursal_id = p_sucursal_id;
    
    -- Registrar movimiento
    INSERT INTO movimientos_inventario (
        producto_id, 
        sucursal_id, 
        tipo_movimiento, 
        cantidad, 
        stock_anterior, 
        stock_nuevo
    )
    VALUES (
        p_producto_id, 
        p_sucursal_id, 
        CASE 
            WHEN p_cantidad > stock_anterior THEN 'entrada'
            WHEN p_cantidad < stock_anterior THEN 'salida'
            ELSE 'ajuste'
        END,
        ABS(p_cantidad - stock_anterior),
        stock_anterior,
        p_cantidad
    );
    
    -- Actualizar stock global en productos
    UPDATE productos
    SET stock = (
        SELECT SUM(stock_actual)
        FROM inventario
        WHERE producto_id = p_producto_id
    )
    WHERE id = p_producto_id;
    
    SELECT 'Stock actualizado correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_agregar_detalle_pedido` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_agregar_detalle_pedido`(
    IN p_pedido_id INT,
    IN p_producto_id INT,
    IN p_cantidad INT,
    IN p_precio_unitario DECIMAL(10,2)
)
BEGIN
    INSERT INTO detalle_pedidos (pedido_id, producto_id, cantidad, precio_unitario)
    VALUES (p_pedido_id, p_producto_id, p_cantidad, p_precio_unitario);
    
    -- Actualizar total del pedido
    UPDATE pedidos
    SET total = (SELECT SUM(cantidad * precio_unitario) FROM detalle_pedidos WHERE pedido_id = p_pedido_id)
    WHERE id = p_pedido_id;
    
    SELECT 'Detalle de pedido agregado correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_agregar_tracking_pedido` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_agregar_tracking_pedido`(
    IN p_pedido_id INT,
    IN p_repartidor_id INT,
    IN p_estado_envio VARCHAR(20)
)
BEGIN
    INSERT INTO envios (pedido_id, repartidor_id, estado_envio)
    VALUES (p_pedido_id, p_repartidor_id, p_estado_envio);
    
    SELECT 'Información de envío agregada correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_buscar_productos` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_buscar_productos`(
    IN p_termino VARCHAR(100),
    IN p_categoria_id INT
)
BEGIN
    IF p_categoria_id > 0 THEN
        SELECT p.*, c.nombre AS categoria, m.nombre AS marca
        FROM productos p
        LEFT JOIN categorias c ON p.categoria_id = c.id
        LEFT JOIN marcas m ON p.marca_id = m.id
        WHERE (p.nombre LIKE CONCAT('%', p_termino, '%') 
               OR p.codigo LIKE CONCAT('%', p_termino, '%')
               OR p.descripcion LIKE CONCAT('%', p_termino, '%'))
              AND p.categoria_id = p_categoria_id;
    ELSE
        SELECT p.*, c.nombre AS categoria, m.nombre AS marca
        FROM productos p
        LEFT JOIN categorias c ON p.categoria_id = c.id
        LEFT JOIN marcas m ON p.marca_id = m.id
        WHERE p.nombre LIKE CONCAT('%', p_termino, '%') 
           OR p.codigo LIKE CONCAT('%', p_termino, '%')
           OR p.descripcion LIKE CONCAT('%', p_termino, '%');
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_cambiar_contrasena` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_cambiar_contrasena`(
    IN p_usuario_id INT,
    IN p_contrasena_actual VARCHAR(255),
    IN p_contrasena_nueva VARCHAR(255)
)
BEGIN
    DECLARE contrasena_guardada VARCHAR(255);
    
    -- Obtener contraseña actual
    SELECT password INTO contrasena_guardada
    FROM usuarios
    WHERE id = p_usuario_id;
    
    -- Verificar contraseña actual
    IF contrasena_guardada = p_contrasena_actual THEN
        -- Actualizar contraseña
        UPDATE usuarios
        SET password = p_contrasena_nueva
        WHERE id = p_usuario_id;
        
        SELECT 'Contraseña actualizada correctamente' AS mensaje;
    ELSE
        SELECT 'La contraseña actual no es correcta' AS mensaje;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_crear_categoria` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_crear_categoria`(
    IN p_nombre VARCHAR(100),
    IN p_codigo VARCHAR(10)
)
BEGIN
    INSERT INTO categorias (nombre, codigo)
    VALUES (p_nombre, p_codigo);
    
    SELECT LAST_INSERT_ID() AS id, 'Categoría creada correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_crear_marca` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_crear_marca`(
    IN p_nombre VARCHAR(100),
    IN p_descripcion TEXT
)
BEGIN
    INSERT INTO marcas (nombre, descripcion)
    VALUES (p_nombre, p_descripcion);
    
    SELECT LAST_INSERT_ID() AS id, 'Marca creada correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_crear_pedido` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_crear_pedido`(
    IN p_cliente_id INT,
    IN p_productos JSON,
    IN p_total DECIMAL(10,2)
)
BEGIN
    DECLARE pedido_id INT;
    DECLARE i INT DEFAULT 0;
    DECLARE items_count INT;
    DECLARE producto_id INT;
    DECLARE cantidad INT;
    DECLARE precio_unit DECIMAL(10,2);

    -- Crear pedido
    INSERT INTO pedidos (usuario_id, estado, total)
    VALUES (p_cliente_id, 'Pendiente', p_total);

    SET pedido_id = LAST_INSERT_ID();

    -- Procesar cada producto del pedido
    SET items_count = JSON_LENGTH(p_productos);

    WHILE i < items_count DO
        SET producto_id = JSON_EXTRACT(p_productos, CONCAT('$[', i, '].producto_id'));
        SET cantidad = JSON_EXTRACT(p_productos, CONCAT('$[', i, '].cantidad'));

        -- Obtener precio del producto
        SELECT precio INTO precio_unit FROM productos WHERE id = producto_id;

        -- Agregar producto al detalle del pedido
        INSERT INTO detalle_pedidos (pedido_id, producto_id, cantidad, precio_unitario)
        VALUES (pedido_id, producto_id, cantidad, precio_unit);

        -- Actualizar stock
        UPDATE productos
        SET stock = stock - cantidad
        WHERE id = producto_id;

        SET i = i + 1;
    END WHILE;

    SELECT pedido_id AS id, 'Pedido creado correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_crear_producto` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_crear_producto`(
    IN p_nombre VARCHAR(150),
    IN p_codigo VARCHAR(50),
    IN p_descripcion TEXT,
    IN p_precio DECIMAL(10,2),
    IN p_stock INT,
    IN p_categoria_id INT
)
BEGIN
    INSERT INTO productos (
        nombre, 
        codigo, 
        descripcion, 
        precio, 
        stock, 
        categoria_id
    )
    VALUES (
        p_nombre, 
        p_codigo, 
        p_descripcion, 
        p_precio, 
        p_stock, 
        p_categoria_id
    );
    
    SELECT LAST_INSERT_ID() AS id, 'Producto creado correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_generar_cupon` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_generar_cupon`(
    IN p_nombre VARCHAR(100),
    IN p_descripcion TEXT,
    IN p_porcentaje_descuento DECIMAL(5,2),
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    INSERT INTO promociones (nombre, descripcion, porcentaje_descuento, fecha_inicio, fecha_fin)
    VALUES (p_nombre, p_descripcion, p_porcentaje_descuento, p_fecha_inicio, p_fecha_fin);
    
    SELECT LAST_INSERT_ID() AS id, 'Cupón generado correctamente' AS mensaje;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_historial_compras_usuario` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_historial_compras_usuario`(
    IN p_usuario_id INT
)
BEGIN
    SELECT p.id AS pedido_id, 
           p.fecha_pedido, 
           p.estado, 
           p.total,
           COUNT(dp.id) AS total_productos
    FROM pedidos p
    JOIN detalle_pedidos dp ON p.id = dp.pedido_id
    JOIN clientes c ON p.usuario_id = c.id
    WHERE c.usuario_id = p_usuario_id
    GROUP BY p.id
    ORDER BY p.fecha_pedido DESC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_informe_ventas` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_informe_ventas`(
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    -- Total de ventas por día
    SELECT DATE(fecha) AS dia, COUNT(*) AS cantidad_pedidos, SUM(total) AS total_ventas
    FROM pedidos
    WHERE fecha BETWEEN p_fecha_inicio AND p_fecha_fin
    GROUP BY DATE(fecha)
    ORDER BY dia;
    
    -- Total de ventas por categoría
    SELECT c.nombre AS categoria, COUNT(dp.id) AS cantidad_productos, SUM(dp.cantidad * dp.precio_unitario) AS total_ventas
    FROM detalle_pedidos dp
    JOIN productos p ON dp.producto_id = p.id
    JOIN categorias c ON p.categoria_id = c.id
    JOIN pedidos pe ON dp.pedido_id = pe.id
    WHERE pe.fecha BETWEEN p_fecha_inicio AND p_fecha_fin
    GROUP BY c.id
    ORDER BY total_ventas DESC;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_iniciar_sesion` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_iniciar_sesion`(
    IN p_email VARCHAR(100),
    IN p_password VARCHAR(255)
)
BEGIN
    DECLARE usuario_id INT;
    
    -- Verificar usuario y contraseña
    SELECT id INTO usuario_id
    FROM usuarios
    WHERE email = p_email AND password = p_password;
    
    IF usuario_id IS NOT NULL THEN
        -- Actualizar último acceso
        UPDATE usuarios
        SET ultimo_acceso = NOW()
        WHERE id = usuario_id;
        
        -- Retornar información del usuario
        SELECT u.id, u.nombre, u.email, u.rol_id, r.nombre AS rol
        FROM usuarios u
        JOIN roles r ON u.rol_id = r.id
        WHERE u.id = usuario_id;
    ELSE
        SELECT 'Usuario o contraseña incorrectos' AS error;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_obtener_historial_tracking` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_obtener_historial_tracking`(
    IN p_pedido_id INT
)
BEGIN
    SELECT e.pedido_id, e.repartidor_id, u.nombre AS repartidor, 
           e.fecha_envio, e.estado_envio
    FROM envios e
    JOIN usuarios u ON e.repartidor_id = u.id
    WHERE e.pedido_id = p_pedido_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_productos_mas_vendidos` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_productos_mas_vendidos`(
    IN p_limite INT
)
BEGIN
    SELECT p.id, p.nombre, p.codigo, p.precio, 
           SUM(dp.cantidad) AS unidades_vendidas
    FROM productos p
    JOIN detalle_pedidos dp ON p.id = dp.producto_id
    JOIN pedidos pe ON dp.pedido_id = pe.id
    WHERE pe.estado = 'Enviado' OR pe.estado = 'Entregado'
    GROUP BY p.id
    ORDER BY unidades_vendidas DESC
    LIMIT p_limite;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_recuperar_contrasena` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_recuperar_contrasena`(
    IN p_email VARCHAR(100)
)
BEGIN
    DECLARE usuario_id INT;
    
    -- Verificar que el usuario existe
    SELECT id INTO usuario_id
    FROM usuarios
    WHERE email = p_email;
    
    IF usuario_id IS NOT NULL THEN
        -- En un entorno real, aquí se enviaría un correo con un enlace para restablecer la contraseña
        -- Para este ejemplo, simplemente confirmamos que el usuario existe
        SELECT 'Solicitud de recuperación de contraseña procesada' AS mensaje;
    ELSE
        SELECT 'No se encontró una cuenta con ese correo electrónico' AS error;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_registrar_usuario` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_registrar_usuario`(
    IN p_nombre VARCHAR(100),
    IN p_email VARCHAR(100),
    IN p_password VARCHAR(255),
    IN p_rut VARCHAR(12),
    IN p_direccion VARCHAR(255),
    IN p_telefono VARCHAR(20)
)
BEGIN
    DECLARE usuario_id INT;
    
    -- Verificar si el email ya existe
    IF EXISTS (SELECT 1 FROM usuarios WHERE email = p_email) THEN
        SELECT 'El correo electrónico ya está registrado' AS error;
    ELSE
        -- Insertar usuario
        INSERT INTO usuarios (
            nombre, 
            email, 
            password, 
            rol_id,
            rut
        )
        VALUES (
            p_nombre, 
            p_email, 
            p_password, 
            3,  -- Cliente
            p_rut
        );
        
        SET usuario_id = LAST_INSERT_ID();
        
        -- Insertar cliente
        INSERT INTO clientes (
            usuario_id, 
            direccion, 
            telefono
        )
        VALUES (
            usuario_id, 
            p_direccion, 
            p_telefono
        );
        
        SELECT usuario_id AS id, 'Usuario registrado correctamente' AS mensaje;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `sp_validar_cupon` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_validar_cupon`(
    IN p_promocion_id INT,
    IN p_total_compra DECIMAL(10,2)
)
BEGIN
    DECLARE descuento DECIMAL(10,2);
    DECLARE promo_activa BOOLEAN;
    DECLARE porcentaje DECIMAL(5,2);
    
    -- Verificar si la promoción existe y está activa
    SELECT activa, porcentaje_descuento INTO promo_activa, porcentaje
    FROM promociones
    WHERE id = p_promocion_id;
    
    IF promo_activa = TRUE THEN
        -- Calcular descuento
        SET descuento = p_total_compra * (porcentaje / 100);
        
        SELECT descuento AS monto_descuento, 'Cupón válido' AS mensaje;
    ELSE
        SELECT 0 AS monto_descuento, 'Cupón no disponible' AS mensaje;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-06-09 20:21:49
