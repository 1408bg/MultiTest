import { ServerResponse, createServer, IncomingMessage, Server } from "http";

/** @typedef {(req: IncomingMessage, res: ServerResponse) => object} Handler */
export class TCPBuilder {
  constructor() {
    /** @type {Map<string, Handler>} */
    this.handlers = new Map();
    /** @type {Server} */
    this.server = createServer(this._handleRequest.bind(this));
  }

  /**
   * @param {string} path
   * @param {Handler} handler
   */
  get(path, handler) {
    this.handlers.set(path, handler);
    return this;
  }

  /**
   * @private
   * @param {object} body
   * @returns {{ buffer: Buffer, contentType: string }}
   */
  _processResponse(body) {
    const buffer = Buffer.from(JSON.stringify(body), 'utf8');
    return { buffer, contentType: 'application/json; charset=utf-8' };
  }

  /**
   * @param {IncomingMessage} req
   * @param {ServerResponse} res
   */
  _handleRequest(req, res) {
    const { url, method } = req;
    if (method !== 'GET' || !url) {
      this._sendError(res, 405, 'Method Not Allowed');
      return;
    }

    const handler = this.handlers.get(url);
    if (!handler) {
      this._sendError(res, 404, 'Not Found');
      return;
    }

    try {
      const result = handler(req, res);
      if (!res.writableEnded) {
        const { buffer, contentType } = this._processResponse(result);
        res.statusCode = 200;
        res.setHeader('Content-Type', contentType);
        res.setHeader('Content-Length', buffer.length);
        res.end(buffer);
      }
    } catch (error) {
      console.error('Request handler error:', error);
      this._sendError(res, 500, 'Internal Server Error');
    }
  }

  /**
   * @private
   * @param {ServerResponse} res
   * @param {number} statusCode
   * @param {string} message
   */
  _sendError(res, statusCode, message) {
    const errorResponse = {
      status: statusCode,
      message: message
    };
    const buffer = Buffer.from(JSON.stringify(errorResponse), 'utf8');
    res.statusCode = statusCode;
    res.setHeader('Content-Type', 'application/json; charset=utf-8');
    res.setHeader('Content-Length', buffer.length);
    res.end(buffer);
  }

  /**
   * @param {number} port
   * @param {() => void} [callback]
   */
  run(port, callback) {
    this.server.listen(port, callback);
    return this;
  }
}
