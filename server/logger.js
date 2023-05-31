const fs = require('fs');
const path = require('path');

const logsDirectory = 'logs';

const createLogsDirectory = () => {
    if (!fs.existsSync(logsDirectory)) {
      fs.mkdirSync(logsDirectory);
    }
  };

const getCurrentDateTime = () => {
const now = new Date();
const year = now.getFullYear();
const month = String(now.getMonth() + 1).padStart(2, '0');
const day = String(now.getDate()).padStart(2, '0');
const hours = String(now.getHours()).padStart(2, '0');
const minutes = String(now.getMinutes()).padStart(2, '0');
const seconds = String(now.getSeconds()).padStart(2, '0');

return `${year}${month}${day}${hours}${minutes}${seconds}`;
};
const getCurrentTime = () => {
  const now = new Date();
  const hours = String(now.getHours()).padStart(2, '0');
  const minutes = String(now.getMinutes()).padStart(2, '0');
  const seconds = String(now.getSeconds()).padStart(2, '0');
  
  return `${hours}:${minutes}:${seconds}`;
  };
const logFileName = getCurrentDateTime().replace(/[-: ]/g, '');

const LogFile = (log,broadcast) => {
    createLogsDirectory();
  
    let logEntry;
    if(!broadcast)logEntry = `${getCurrentTime()} - LOG - ${log}`;
    if(broadcast) logEntry = `${getCurrentTime()} - LOGB- ${log}`;
    const logFilePath = path.join(__dirname, logsDirectory, `${logFileName}.txt`);

    const cleanedLogEntry = logEntry.replace(/\\/g, '');
    fs.appendFile(logFilePath, cleanedLogEntry + '\n', (err) => {
      if (err)console.error('LOGGIN ERROR', err);
      
    });
  };
const ErrorFile = (log) => {
    createLogsDirectory();
  
    const logEntry = `${getCurrentTime()} - ERR - ${log}`;
    
    const logFilePath = path.join(__dirname, logsDirectory, `${logFileName}.txt`);

    const cleanedLogEntry = logEntry.replace(/\\/g, '');
    fs.appendFile(logFilePath, cleanedLogEntry + '\n', (err) => {
      if (err)console.error('LOGGIN ERROR', err);
      
    });
  };

module.exports ={LogFile,ErrorFile}