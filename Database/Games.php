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
  
  $sql = "INSERT INTO games (gamenum, winningnum)
  VALUES ('" . $data["GAMENUM"]. "','" . $data["WINNINGNUM"]. "')";

  if ($conn->query($sql) === TRUE) {
    echo "New record created successfully";
  } else {
    echo "Error: " . $sql . "<br>" . $conn->error;
  }
} 

if ($_SERVER["REQUEST_METHOD"] == "GET"){
  // Give count of games
  if (isset($_GET["count"])){
    $sql = "SELECT COUNT(*) FROM games";

    $result = $conn->query($sql);

    if ($result){
      $row = $result->fetch_row();
      echo $row[0];
    }
  } // Give winning number from gamenum
  else if (isset($_GET["gamenum"])){
    $sql = "SELECT WINNINGNUM FROM games WHERE gamenum=\"" . $_GET["gamenum"] . "\"";

    $result = $conn->query($sql);
    if ($result){
      $row = $result->fetch_row();
      echo $row[0];
    }
  else{
    header("HTTP/1.1 400 Bad Request");
  }
  }
}


$conn->close();
?>