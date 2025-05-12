import { createSocket } from 'dgram';

/** @typedef {(data: string, info: import('dgram').RemoteInfo) => object|null} Handler */
export class UDPBuilder {
  /** @param {string} token */
  constructor(token = '\n') {
    /** @type {Map<string, Handler>} */
    this.handlers = new Map();
    this.server = createSocket('udp4');
    this.server.on('message', (buffer, remote) => {
      const message = buffer.toString('utf-8');
      const newlineIndex = message.indexOf(token);
      if (newlineIndex === -1) return;

      const event = message.slice(0, newlineIndex);
      const data = message.slice(newlineIndex + 1);
      const handler = this.handlers.get(event);
      if (!handler) return;
      
      try {
        const result = handler(data, remote);
        if (!result) return;
        
        const response = JSON.stringify(result);
        this.send(response, remote.port, remote.address);
      } catch (err) {
        const errorResponse = {
          error: err.message,
          timestamp: Date.now()
        };
        console.error(`[ERROR] - ${new Date().toLocaleString()}\n${event}: ${err}`);
        this.send(JSON.stringify(errorResponse), remote.port, remote.address);
      }
    });
  }

  /**
   * @param {string} response
   * @param {number} port
   * @param {string} address
   */
  send(response, port, address) {
    const buffer = Buffer.from(response, 'utf-8');
    this.server.send(buffer, 0, buffer.length, port, address);
  }

  /**
   * @param {string} name
   * @param {Handler} handler
   */
  on(name, handler) {
    if (this.handlers.has(name)) throw new Error('Event already registered');
    this.handlers.set(name, handler);
    return this;
  }

  /**
   * @param {number} port
   * @param {(server: UDPBuilder) => void} [callback] 
   */
  run(port, callback) {
    this.server.on('listening', () => callback(this));
    this.server.bind(port);
    return this;
  }
}
