// This uses child_process to run Butler
// https://nodejs.org/api/child_process.html#child_process_child_process_spawn_command_args_options
var path = require("path");
var execFile = require("child_process").execFile;

// NodeJS application that
// butler push mygame user/mygame:win32 --userversion 1.1.0(-final)
// See README for version number construction details

// EXAMPLE projectstart = new Date(2022, 8, 28, 13, 0, 0, 0).valueOf();
// This would be 2022 September 28, 1:00:00.000 PM

// DAYLIGHT SAVINGS TIME adjustment. Noon = 1:00 after the time change.
const projectstart = new Date(2026, 0, 21, 0, 0, 0, 0).valueOf();
/** How many weeks for prototyping? Appends '-proto' on the version number until that point. */
const planningweeks = 2;
/** How many weeks for production? Appends '-alpha' on the version number until that point. */
const productionweeks = 12;
/** How many weeks for polish? Appends '-beta' on the version number until that point, '-final' afterward. */
const polishweeks = 2;

const username = "cscc-game-projects";

// ATTENTION BUILD MANAGER: YOU MAY NEED TO MODIFY THESE LINES FOR YOUR GAME
// ESPECIALLY IF YOU CHANGE THE GAME URL ON itch.io
const gamename = "game-url-stub-goes-here";
const butler_command_file = "./butler/versions/15.24.0/win/butler.exe";
// END OF ATTENTION BLOCK

function CurrentVersionString() {
  const now = Date.now();
  var msLeft = now - projectstart;
  const msPerMinute = 1000 * 60;
  const msPerHour = msPerMinute * 60;
  const msPerDay = msPerHour * 24;
  const msPerWeek = msPerDay * 7;
  const fullWeeks = Math.floor(msLeft / msPerWeek).toString(10);
  msLeft -= fullWeeks * msPerWeek;
  const fullDays = Math.floor(msLeft / msPerDay).toString(10);
  msLeft -= fullDays * msPerDay;
  const fullHours = Math.floor(msLeft / msPerHour)
    .toString(10)
    .padStart(2, "0");
  msLeft -= fullHours * msPerHour;
  const fullMinutes = Math.floor(msLeft / msPerMinute)
    .toString(10)
    .padStart(2, "0");
  const base_string = `1.${fullWeeks}.${fullDays}${fullHours}${fullMinutes}`;
  if (fullWeeks < planningweeks) {
    return base_string + "-proto";
  } else if (fullWeeks < planningweeks + productionweeks) {
    return base_string + "-alpha";
  } else if (fullWeeks < planningweeks + productionweeks + polishweeks) {
    return base_string + "-beta";
  }
  return base_string + "-final";
}

function UploadTeamGame(platform, version_string, cb) {
  var exePath = path.resolve(__dirname, butler_command_file);
  const butler_command_args = [
    "push",
    platform,
    `${username}/${gamename}:${platform}`,
    `--userversion`,
    version_string,
  ];
  //const butler_command = `/butler/versions/15.24.0/win/butler.exe push ${platform} ${username}/${gamename}:${platform} --userversion ${version_string}`;
  console.log(`RUNNING ${exePath} ${JSON.stringify(butler_command_args)}`);
  execFile(exePath, butler_command_args, (error, stdout, stderr) => {
    if (error) {
      console.error(`exec error: ${error}`);
      console.log(`ran from ${exePath}`);
      console.log(`stdout: ${stdout}`);
      console.error(`stderr: ${stderr}`);
      return;
    }

    console.log(`stdout: ${stdout}`);
    console.log(
      `stderr (included for information only; run was successful): ${stderr}`,
    );
    console.log(`FINISHED ${JSON.stringify(butler_command_args)}\n\n`);
    cb(version_string);
  });
}
function AllDone(version_string) {
  console.log(`\nWoo! All done uploading ${version_string}!`);
}
function ButlerUploadLinux(version_string) {
  UploadTeamGame("linux", version_string, AllDone);
}
function ButlerUploadMac(version_string) {
  UploadTeamGame("mac", version_string, ButlerUploadLinux);
}
function ButlerUploadWin(version_string) {
  UploadTeamGame("win", version_string, ButlerUploadMac);
}
/* REMOVED IN 1.4.0 -- win32 function ButlerUploadWin32(version_string) {
  UploadTeamGame("win32", version_string, ButlerUploadWin);
} */
/* Example for web HTML game 
function ButlerUploadHTMLGame(version_string) {
  UploadTeamGame("web", version_string, AllDone);
}
*/
function RunButlerUploader() {
  const now_date = new Date(Date.now());
  const proj_date = new Date(projectstart);
  const version_string = CurrentVersionString();
  console.log("\nCalculating version number...");
  console.log(`now = ${now_date.toDateString()} @ ${now_date.toTimeString()}`);
  console.log(
    `project start = ${proj_date.toDateString()} @ ${proj_date.toTimeString()}`,
  );
  console.log("So, version number is " + version_string);
  ButlerUploadWin(version_string);

  /* REMOVED IN 1.4.0 -- win32 const args = process.argv;
  // Restore this if passing additional arguments from the bat / sh side.
  if (args.length > 2) {
    console.log(
      "No Windows-32 build being supplied. Poor forsaken gamers...\n\n"
    );
    ButlerUploadWin(version_string);
  } else {
    console.log(
      "You are awesome for providing a 32-bit Windows binary for deprived gamers!\n\n"
    );
    ButlerUploadWin32(version_string);
  } */

  /* Example for web HTML game
  ButlerUploadHTMLGame(version_string);
    */
}
RunButlerUploader();
