-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: musyncdevdb
-- ------------------------------------------------------
-- Server version	9.6.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '12f1110c-0283-11f1-9fc7-2a78e37884c4:1-69';

--
-- Table structure for table `OAuthTokens`
--

DROP TABLE IF EXISTS `OAuthTokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OAuthTokens` (
  `OAuthId` int NOT NULL AUTO_INCREMENT,
  `Provider` varchar(100) NOT NULL,
  `RefreshToken` varchar(100) DEFAULT NULL,
  `UserId` int DEFAULT NULL,
  PRIMARY KEY (`OAuthId`),
  KEY `OAuthTokens_Users_FK` (`UserId`),
  CONSTRAINT `OAuthTokens_Users_FK` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `RememberTokens`
--

DROP TABLE IF EXISTS `RememberTokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RememberTokens` (
  `RememberId` int NOT NULL AUTO_INCREMENT,
  `CreationDate` datetime NOT NULL,
  `ExpiryDate` datetime NOT NULL,
  `TokenHash` varchar(100) NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`RememberId`),
  UNIQUE KEY `RememberTokens_UNIQUE` (`TokenHash`),
  KEY `RememberToken_Users_FK` (`UserId`),
  CONSTRAINT `RememberToken_Users_FK` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Sessions`
--

DROP TABLE IF EXISTS `Sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Sessions` (
  `SessionId` int NOT NULL AUTO_INCREMENT,
  `CreationDate` datetime NOT NULL,
  `ExpiryDate` datetime NOT NULL,
  `UserId` int NOT NULL,
  `SessionHash` varchar(100) NOT NULL,
  PRIMARY KEY (`SessionId`),
  UNIQUE KEY `Sessions_UNIQUE` (`SessionHash`),
  KEY `Sessions_Users_FK` (`UserId`),
  CONSTRAINT `Sessions_Users_FK` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Users`
--

DROP TABLE IF EXISTS `Users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Users` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_as_cs NOT NULL,
  `Password` varchar(100) NOT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping routines for database 'musyncdevdb'
--
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-15 14:28:18
