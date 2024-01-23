<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "roulette";

function get_tickets($sql,$conn){
  $result = $conn->query($sql);

  $rows = array();
  if ($result->num_rows > 0) {
    // output data of each row
    while($row = $result->fetch_assoc()) {
      array_push($rows, json_encode($row));
    }

    // Unity's JSON utility doesn't support top-level JSON arrays. This simply wraps it in an object.
    echo "{\"tickets\": [";
    for ($i=0;$i < count($rows);$i++){
      echo $rows[$i];
      if ($i != count($rows)-1)
        echo ",";
        
    }
    echo "]}";

  } else {
    header("HTTP/1.1 400 Bad Request");
    echo "0 results";
  }
}

function get_ticket_count($conn){
  $sql = "SELECT COUNT(*) FROM tickets";

  $result = $conn->query($sql);

  if ($result){
    $row = $result->fetch_row();
    echo $row[0];
  }
  else{
    header("HTTP/1.1 400 Bad Request");
    echo "0 results";
  }
}

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

if (isset($_GET["count"])){
  get_ticket_count($conn);
}
else{
  $sql = "SELECT * FROM tickets";
  if (isset($_GET['id'])){
    $sql = "SELECT * FROM tickets WHERE id=\"" . $_GET['id'] . "\"";
  }
  get_tickets($sql, $conn);
}

$conn->close();
?>