import tcp from './servers/tcp.js';
import udp from './servers/udp.js';

if (tcp !== null && udp !== null) {
  console.log('ok');
}