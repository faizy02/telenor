#!/bin/bash

# Different functions used at the end of the script
function clusterstop {

  echo "Stopping Cluster $1"

  Cluster stop $1
  while :
  do
    sleep 2s
    clusterstatus="$(Cluster list | grep $1)"
    if (echo "$clusterstatus" | grep -q "DOWN"); then
        echo "$1 Stopped";
        echo "----------------";
        break;
    else
        echo "$1 is still running";
    fi
  done
}

function killprocess {
  echo "Killing remaining 501 proceses"
  process501="$(ps -ef | grep 501)";
  for i in `echo "$process501" | awk '{ print $2 }'`;
  do
    processrow="$(echo "$process501" | grep $i)";
    if (echo "$processrow" | grep -q "bash\|root");then
      echo "$processrow not killed $i";
    else
      kill -9 $i
    fi
  done
  echo "_---------------------------------"
  echo "Killing remaining tertio processes"
  processtertio="$(ps -ef |grep tertio)"
  for i in `echo "$processtertio" | awk '{ print $2 }'`;
  do
    processrow="$(echo "$process501" | grep $i)";
    if (echo "$processrow" | grep -q "bash\|root");then
      echo "$processrow not killed $i";
    else
      kill -9 $i
    fi
  done
  
}


function tertio_down {

echo "TERTIO SHUTTING DOWN..."

source ~/.bash_profile

clusterstop tertio.PA_HLRa
clusterstop tertio.PA_CC
clusterstop tertio.SA_ORACLE
clusterstop tertio.SA_EAI
clusterstop tertio.PA_general
clusterstop tertio.PA_CBS
clusterstop tertio.PA_CBS1
clusterstop tertio.SA_BSA
clusterstop tertio.PA_internal
clusterstop tertio.stps.20a
clusterstop tertio.stps.20b
clusterstop tertio.core
Cluster stop tertio.server

sleep 3s

Cluster list

sleep 3s

killprocess
}

# Warning menu for user to whether quit or continue
Warning='Do you want to continue to SHUT DOWN TERTIO: '
echo "$Warning"
OPTIONS="yes no"
 select opt in $OPTIONS; do
     if [ "$opt" = "yes" ]; then
      	echo script executing...
      	tertio_down
      	break;
     elif [ "$opt" = "no" ]; then
      	echo script not executed
	exit
     else
      	clear
      	echo bad option
     fi
 done