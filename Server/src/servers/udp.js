import { UDPBuilder } from '../../builder/udp-builder.js';
import config from '../config.js';

const PORT = config.UDP_PORT;
const TICK_RATE = 1000 / 60;

const positions = new Map();
/** @type {Map<string, RemoteInfo>} */
const clients = new Map();

const udp = new UDPBuilder()
  .on('move', (data) => {
    const [clientId, vector] = data.split('|');
    const [dx, dy] = vector.split(',').map(Number);

    if (!positions.has(clientId)) positions.set(clientId, { x: 0, y: 0 });
    const pos = positions.get(clientId);
    pos.x += dx;
    pos.y += dy;
    
    return {
      type: 'move_processed',
      clientId,
      position: pos,
      timestamp: Date.now()
    };
  })
  .on('register', (data, remote) => {
    const clientId = data.trim();
    clients.set(clientId, remote);
    if (!positions.has(clientId)) positions.set(clientId, { x: 0, y: 0 });
    
    const users = {};
    for (const [id, pos] of positions) users[id] = pos;
    
    return {
      type: 'connect',
      clientId,
      users,
      timestamp: Date.now()
    };
  })
  .run(PORT, (server) => {
    setInterval(() => {
      const state = {};
      for (const [id, pos] of positions) state[id] = pos;
    
      for (const [_, remote] of clients) {
        const syncData = {
          type: 'sync',
          timestamp: Date.now(),
          state
        };
        server.send(JSON.stringify(syncData), remote.port, remote.address);
      }
    }, TICK_RATE);
    console.log(`UDP sync server on ${PORT}`);
  });

export default udp;