import { TCPBuilder } from "../../builder/tcp-builder.js";
import config from '../config.js';

const PORT = config.TCP_PORT;

const tcp = new TCPBuilder()
  .get('/', () => ({
    data: {
      status: "ok",
      timestamp: Date.now()
    },
  }))
  .run(PORT, () => {
    console.log(`TCP status server on ${PORT}`);
  });

export default tcp;