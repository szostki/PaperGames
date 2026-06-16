window.PeerJSInterop = {
    _peer: null,
    _connections: {},
    _dotnet: null,

    initHost(lobbyCode, dotnet) {
        return new Promise((resolve, reject) => {
            this._dotnet = dotnet;
            this._peer = new Peer(lobbyCode);
            this._peer.on('open', () => {
                this._peer.on('connection', conn => this._setupConnection(conn));
                resolve();
            });
            this._peer.on('error', err => {
                dotnet.invokeMethodAsync('OnPeerError', err.message || err.type);
                reject(err.message || err.type);
            });
        });
    },

    initGuest(dotnet) {
        return new Promise((resolve, reject) => {
            this._dotnet = dotnet;
            this._peer = new Peer();
            this._peer.on('open', id => resolve(id));
            this._peer.on('error', err => {
                dotnet.invokeMethodAsync('OnPeerError', err.message || err.type);
                reject(err.message || err.type);
            });
        });
    },

    connectTo(hostId) {
        return new Promise((resolve, reject) => {
            const conn = this._peer.connect(hostId, { reliable: true });
            const timeout = setTimeout(() => reject('Connection timed out'), 15000);
            conn.on('open', () => {
                clearTimeout(timeout);
                this._setupConnection(conn);
                resolve();
            });
            conn.on('error', err => {
                clearTimeout(timeout);
                reject(err.message || String(err));
            });
        });
    },

    _setupConnection(conn) {
        this._connections[conn.peer] = conn;
        conn.on('data', data => {
            this._dotnet.invokeMethodAsync('OnMessageReceived', conn.peer, data);
        });
        conn.on('close', () => {
            delete this._connections[conn.peer];
            this._dotnet.invokeMethodAsync('OnPeerDisconnected', conn.peer);
        });
        this._dotnet.invokeMethodAsync('OnPeerConnected', conn.peer);
    },

    sendTo(peerId, json) {
        const conn = this._connections[peerId];
        if (conn && conn.open) conn.send(json);
    },

    sendToAll(json) {
        for (const conn of Object.values(this._connections)) {
            if (conn.open) conn.send(json);
        }
    },

    destroy() {
        if (this._peer) {
            this._peer.destroy();
            this._peer = null;
        }
        this._connections = {};
    },

    generateQR(elementId, text) {
        const canvas = document.getElementById(elementId);
        if (canvas && typeof QRCode !== 'undefined') {
            QRCode.toCanvas(canvas, text, { width: 180, margin: 1 }, err => {
                if (err) console.error('QR error:', err);
            });
        }
    },

    getElementRect(element) {
        const r = element.getBoundingClientRect();
        return { left: r.left, top: r.top, width: r.width, height: r.height };
    }
};
