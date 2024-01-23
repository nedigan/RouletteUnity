<?php
header("Content-Type: text/plain");
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "roulette";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

if ($_SERVER["REQUEST_METHOD"] == "POST") {
  $data = json_decode(file_get_contents('php://input'), true); // Gets raw json data and converts to associative array (dictionary)
  
  $sql = "INSERT INTO tickets (id, bets, gamenum, date, voucher, credit)
  VALUES ('" . $data["ID"]. "','" . $data["BETS"]. "','" . $data["GAMENUM"]. "','" . $data["DATE"]. "','" . $data["VOUCHER"]. "','" . $data["CREDIT"] . "')";

  if ($conn->query($sql) === TRUE) {
    echo "New record created successfully";
  } else {
    echo "Error: " . $sql . "<br>" . $conn->error;
  }
} 
else if ($_SERVER["REQUEST_METHOD"] == "PUT"){
  $data = file_get_contents('php://input');

  $sql = "UPDATE `tickets` SET `REDEEMED` = '1' WHERE `tickets`.`ID` = '$data';";
  if ($conn->query($sql)) {
    echo "Ticket updated successfully!";
  } else {
    echo "Error: " . $sql . "<br>" . $conn->error;
  }
}


$conn->close();
?>