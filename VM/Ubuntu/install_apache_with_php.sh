#!/bin/bash

apt-get -y update

logger "Installing Apache WebSite"

# Set up a silent install of MySQL
dbpass=$1

export DEBIAN_FRONTEND=noninteractive
echo mysql-server-5.6 mysql-server/root_password password $dbpass | debconf-set-selections
echo mysql-server-5.6 mysql-server/root_password_again password $dbpass | debconf-set-selections

# Install the LAMP stack 
apt-get -y install apache2 mysql-server php php-mysql libapache2-mod-php php-mcrypt 
# Install additional packages
apt-get -y install mc lynx 



# put main page on WebSite
echo "<h2>Welcome to Ubuntu On Azure.</h2> <?php printf('SERVER_ADDR: %s ', $_SERVER['SERVER_ADDR']);printf('HTTP_HOST: %s ',$_SERVER['HTTP_HOST']);printf('REMOTE_ADDR: %s',$_SERVER['REMOTE_ADDR']);?>" | sudo tee /var/www/html/index.php
# Restart Apache
apachectl restart

logger "Done installing Apache WebSite;"