-- --------------------------------------------------------
-- 호스트:                          127.0.0.1
-- 서버 버전:                        11.1.2-MariaDB - mariadb.org binary distribution
-- 서버 OS:                        Win64
-- HeidiSQL 버전:                  12.3.0.6589
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- 테이블 01database.accountdailyshopdb 구조 내보내기
CREATE TABLE IF NOT EXISTS `accountdailyshopdb` (
  `UID` varchar(15) NOT NULL,
  `ShopListjson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`ShopListjson`)),
  PRIMARY KEY (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

-- 테이블 01database.accountinfojson 구조 내보내기
CREATE TABLE IF NOT EXISTS `accountinfojson` (
  `UID` varchar(12) NOT NULL,
  `accountjson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin DEFAULT NULL CHECK (json_valid(`accountjson`)),
  PRIMARY KEY (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

-- 테이블 01database.dailyshopdb 구조 내보내기
CREATE TABLE IF NOT EXISTS `dailyshopdb` (
  `PrimaryID` int(11) NOT NULL AUTO_INCREMENT,
  `itemid` varchar(16) NOT NULL,
  `Amount` int(11) NOT NULL,
  `needitemID` varchar(16) NOT NULL,
  `price` int(11) NOT NULL,
  `weight` tinyint(4) NOT NULL,
  PRIMARY KEY (`PrimaryID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

-- 이벤트 01database.InitializeDailyShopDB 구조 내보내기
DELIMITER //
CREATE EVENT `InitializeDailyShopDB` ON SCHEDULE EVERY 1 DAY STARTS '2024-02-04 01:24:43' ON COMPLETION NOT PRESERVE ENABLE COMMENT '상점 데이터베이스를 초기화하는 스케줄러' DO TRUNCATE accountDailyShopDB//
DELIMITER ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
