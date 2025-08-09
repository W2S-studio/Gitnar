import { spawn } from 'child_process';
import { platform } from 'os';

function openBrowser(url) {
    let command;
    let args = [];

    switch (platform()) {
        case 'darwin': // macOS
            command = 'open';
            args = [url];
            break;
        case 'win32': // Windows
            command = 'cmd';
            args = ['/c', 'start', '""', url];
            break;
        default: // Linux and others
            command = 'xdg-open';
            args = [url];
            break;
    }

    const browserProcess = spawn(command, args, {
        detached: true,
        stdio: 'ignore'
    });

    browserProcess.unref();
    console.log(`🌐 Opening browser: ${url}`);
}

console.log('🚀 Starting development environment...');

const dockerProcess = spawn('docker-compose', ['up', '--build'], {
    stdio: 'inherit'
});

setTimeout(() => {
    openBrowser('https://localhost');
}, 20000);

process.on('SIGINT', () => {
    console.log('\n⏹️ Shutting down...');
    dockerProcess.kill('SIGINT');
    process.exit(0);
});

process.on('SIGTERM', () => {
    dockerProcess.kill('SIGTERM');
    process.exit(0);
});

dockerProcess.on('close', (code) => {
    console.log(`Docker process exited with code ${code}`);
    process.exit(code);
});