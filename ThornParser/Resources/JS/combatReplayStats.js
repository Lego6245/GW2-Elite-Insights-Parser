/*jshint esversion: 6 */

var compileCombatReplay = function () {
    Vue.component("combat-replay-damage-stats-component", {
        props: ["time", "playerindex"],
        template: `${tmplCombatReplayDamageTable}`,
        data: function () {
            return {
                damageMode: 1
            };
        },
        created() {
            var i, cacheID;
            for (var j = 0; j < this.targets.length; j++) {
                var activeTargets = [j];
                cacheID = 0 + '-';
                cacheID += getTargetCacheID(activeTargets);
                // compute dps for all players
                for (i = 0; i < logData.players.length; i++) {
                    computePlayerDPS(logData.players[i], this.graph[i], 0, null, activeTargets, cacheID + '-' + 0);
                }
            }
            cacheID = 0 + '-';
            cacheID += getTargetCacheID(this.targets);
            // compute dps for all players
            for (i = 0; i < logData.players.length; i++) {
                computePlayerDPS(logData.players[i], this.graph[i], 0, null, this.targets, cacheID + '-' + 0);
            }
        },
        mounted() {
            initTable("#combat-replay-dps-table", 2, "desc");
        },
        updated() {
            updateTable("#combat-replay-dps-table");
        },
        computed: {
            phase: function () {
                return logData.phases[0];
            },
            targets: function () {
                return this.phase.targets;
            },
            graph: function () {
                return graphData.phases[0].players;
            },
            tableData: function () {
                var rows = [];
                var cols = [];
                var sums = [];
                var total = [];
                var curTime = Math.floor(this.time / 1000);
                var nextTime = curTime + 1;
                var dur = Math.floor(this.phase.end - this.phase.start);
                if (nextTime == dur + 1 && this.phase.needsLastPoint) {
                    nextTime = this.phase.end - this.phase.start;
                }
                var i, j;
                for (j = 0; j < this.targets.length; j++) {
                    var target = logData.targets[this.targets[j]];
                    cols.push(target);
                }
                for (i = 0; i < this.graph.length; i++) {
                    var cacheID, data, cur, next;
                    var player = logData.players[i];
                    var graphData = this.graph[i];
                    var dps = [];
                    // targets
                    for (j = 0; j < this.targets.length; j++) {
                        var activeTargets = [j];
                        cacheID = 0 + '-';
                        cacheID += getTargetCacheID(activeTargets);
                        data = computePlayerDPS(player, graphData, 0, null, activeTargets, cacheID + '-' + 0).total.target;
                        cur = data[curTime];
                        next = data[curTime + 1];
                        if (typeof next !== "undefined") {
                            dps[2 * j] = cur + (this.time / 1000 - curTime) * (next - cur)/(nextTime - curTime);
                        } else {
                            dps[2 * j] = cur;
                        }
                        dps[2 * j + 1] = dps[2 * j] / (Math.max(this.time / 1000, 1));
                    }
                    cacheID = 0 + '-';
                    cacheID += getTargetCacheID(this.targets);
                    data = computePlayerDPS(player, graphData, 0, null, this.targets, cacheID + '-' + 0).total.total;
                    cur = data[curTime];
                    next = data[curTime + 1];
                    if (typeof next !== "undefined") {
                        dps[2 * j] = cur + (this.time / 1000 - curTime) * (next - cur)/(nextTime - curTime);
                    } else {
                        dps[2 * j] = cur;
                    }
                    dps[2 * j + 1] = dps[2 * j] / (Math.max(this.time / 1000, 1));
                    for (j = 0; j < dps.length; j++) {
                        total[j] = (total[j] || 0) + dps[j];
                    }
                    rows.push({
                        player: player,
                        dps: dps
                    });
                }
                sums.push({
                    name: "Total",
                    dps: total
                });
                var res = {
                    cols: cols,
                    rows: rows,
                    sums: sums
                };
                return res;
            }
        }
    });

    Vue.component("combat-replay-player-buffs-stats-component", {
        props: ["playerindex", "time"],
        template: `${tmplCombatReplayPlayerBuffStats}`,
        computed: {
            boons: function () {
                var hash = new Set();
                for (var i = 0; i < logData.boons.length; i++) {
                    hash.add(logData.boons[i]);
                }
                return hash;
            },
            offs: function () {
                var hash = new Set();
                for (var i = 0; i < logData.offBuffs.length; i++) {
                    hash.add(logData.offBuffs[i]);
                }
                return hash;
            },
            defs: function () {
                var hash = new Set();
                for (var i = 0; i < logData.defBuffs.length; i++) {
                    hash.add(logData.defBuffs[i]);
                }
                return hash;
            },
            conditions: function () {
                var hash = new Set();
                for (var i = 0; i < logData.conditions.length; i++) {
                    hash.add(logData.conditions[i]);
                }
                return hash;
            },
            buffData: function () {
                return logData.players[this.playerindex].details.boonGraph[0];
            },
            data: function () {
                var res = {
                    offs: [],
                    defs: [],
                    boons: [],
                    conditions: [],
                    enemies: [],
                    others: [],
                    consumables: []
                };
                for (var i = 0; i < this.buffData.length; i++) {
                    var data = this.buffData[i];
                    var id = data.id;
                    if (id < 0) {
                        continue;
                    }
                    var arrayToFill = [];
                    var buff = findSkill(true, id);
                    if (buff.consumable) {
                        arrayToFill = res.consumables;
                    } else if (buff.enemy) {
                        arrayToFill = res.enemies;
                    } else if (this.boons.has(id)) {
                        arrayToFill = res.boons;
                    } else if (this.offs.has(id)) {
                        arrayToFill = res.offs;
                    } else if (this.defs.has(id)) {
                        arrayToFill = res.defs;
                    } else if (this.conditions.has(id)) {
                        arrayToFill = res.conditions;
                    } else {
                        arrayToFill = res.others;
                    }
                    var val = data.states[0][1];
                    var t = this.time / 1000;
                    for (var j = 1; j < data.states.length; j++) {
                        if (data.states[j][0] < t) {
                            val = data.states[j][1];
                        } else {
                            break;
                        }
                    }
                    if (val > 0) {
                        arrayToFill.push({
                            state: val,
                            buff: buff
                        });
                    }
                }
                return res;
            }
        }
    });

    Vue.component("combat-replay-player-status-component", {
        props: ["playerindex", "time"],
        template: `${tmplCombatReplayPlayerStatus}`,
        computed: {
            player: function () {
                return logData.players[this.playerindex];
            },
            status: function () {
                var crData = animator.playerData.get(this.player.combatReplayID);
                var icon = crData.getIcon(this.time);
                return icon === deadIcon ? 0 : icon === downIcon ? 1 : 2;
            },
        }
    });

    Vue.component("combat-replay-player-rotation-component", {
        props: ["playerindex", "time"],
        template: `${tmplCombatReplayPlayerRotation}`,
        computed: {
            player: function () {
                return logData.players[this.playerindex];
            },
            playerRotation: function () {
                return this.player.details.rotation[0];
            },
            rotation: function () {
                var res = {
                    current: null,
                    nexts: []
                };
                var time = this.time / 1000.0;
                var j, next;
                for (var i = 0; i < this.playerRotation.length; i++) {
                    count = 0;
                    var item = this.playerRotation[i];
                    var x = item[0];
                    var skillId = item[1];
                    var endType = item[3];
                    if (item[2] < 1e-2) {
                        continue;
                    }
                    var duration = Math.round(item[2] / 1000.0);
                    var skill = findSkill(false, skillId);
                    if ((x <= time && time <= x + duration) || (time <= x && i > 0)) {
                        var offset = 0;
                        if ((x <= time && time <= x + duration)) {
                            res.current = {
                                skill: skill,
                                end: endType
                            };
                            offset = 1;
                        }
                        for (j = i + offset; j < this.playerRotation.length; j++) {
                            next = this.playerRotation[j];
                            if (next[2] < 1e-2) {
                                continue;
                            }
                            res.nexts.push({
                                skill: findSkill(false, next[1]),
                                end: next[3]
                            });
                            if (res.nexts.length == 3) {
                                break;
                            }
                        }
                        break;
                    } else if (time <= x) {
                        for (j = i; j < this.playerRotation.length; j++) {
                            next = this.playerRotation[j];
                            if (next[2] < 1e-2) {
                                continue;
                            }
                            res.nexts.push({
                                skill: findSkill(false, next[1]),
                                end: next[3]
                            });
                            if (res.nexts.length == 3) {
                                break;
                            }
                        }
                        break;
                    }
                }
                return res;
            },
        }
    });

    Vue.component("combat-replay-player-stats-component", {
        props: ["playerindex", "time"],
        template: `${tmplCombatReplayPlayerStats}`
    });

    Vue.component("combat-replay-damage-data-component", {
        template: `${tmplCombatReplayDamageData}`,
        props: ["time", "selectedplayer", "selectedplayerid"],
        computed: {
            playerindex: function () {
                if (this.selectedplayer) {
                    for (var i = 0; i < logData.players.length; i++) {
                        if (logData.players[i].combatReplayID == this.selectedplayerid) {
                            return i;
                        }
                    }
                }
                return -1;
            }
        }
    });

    Vue.component("combat-replay-status-data-component", {
        template: `${tmplCombatReplayStatusData}`,
        props: ["time", "selectedplayer", "selectedplayerid"],
        data: function() {
            return {
                details: false
            };
        },
        updated() {
            animator.controlledByHTML = this.details;
            animator.draw();
        },
        computed: {
            playerindex: function () {
                if (this.selectedplayer) {
                    for (var i = 0; i < logData.players.length; i++) {
                        if (logData.players[i].combatReplayID == this.selectedplayerid) {
                            return i;
                        }
                    }
                }
                return -1;
            },
            groups: function () {
                var res = [];
                var i = 0;
                for (i = 0; i < logData.players.length; i++) {
                    var playerData = logData.players[i];
                    if (playerData.isConjure) {
                        continue;
                    }
                    if (!res[playerData.group]) {
                        res[playerData.group] = [];
                    }
                    res[playerData.group].push(playerData);
                }
                return res;
            }
        }
    });
};
