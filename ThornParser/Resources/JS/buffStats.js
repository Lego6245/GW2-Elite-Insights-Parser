/*jshint esversion: 6 */

var compileBuffStats = function () {
    Vue.component("personal-buff-table-component", {
        props: ['phaseindex', 'playerindex'],
        template: `${tmplPersonalBuffTable}`,
        data: function () {
            return {
                bases: [],
                mode: "Warrior",
                cache: new Map(),
                specToBase: specToBase
            };
        },
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            orderedSpecs: function () {
                var res = [];
                var aux = new Set();
                for (var i = 0; i < specs.length; i++) {
                    var spec = specs[i];
                    var pBySpec = [];
                    for (var j = 0; j < logData.players.length; j++) {
                        if (logData.players[j].profession === spec && logData.phases[0].persBuffStats[j].data.length > 0) {
                            pBySpec.push(j);
                        }
                    }
                    if (pBySpec.length) {
                        aux.add(specToBase[spec]);
                        res.push({
                            ids: pBySpec,
                            name: spec
                        });
                    }
                }
                this.bases = [];
                var _this = this;
                aux.forEach(function (value, value2, set) {
                    _this.bases.push(value);
                });
                this.mode = this.bases[0];
                return res;
            },
            data: function () {
                if (this.cache.has(this.phaseindex)) {
                    return this.cache.get(this.phaseindex);
                }
                var res = [];
                for (var i = 0; i < this.orderedSpecs.length; i++) {
                    var spec = this.orderedSpecs[i];
                    var dataBySpec = [];
                    for (var j = 0; j < spec.ids.length; j++) {
                        dataBySpec.push({
                            player: logData.players[spec.ids[j]],
                            data: this.phase.persBuffStats[spec.ids[j]]
                        });
                    }
                    res.push(dataBySpec);
                }
                this.cache.set(this.phaseindex, res);
                return res;
            },
            buffs: function () {
                var res = [];
                for (var i = 0; i < this.orderedSpecs.length; i++) {
                    var spec = this.orderedSpecs[i];
                    var data = [];
                    for (var j = 0; j < logData.persBuffs[spec.name].length; j++) {
                        data.push(findSkill(true, logData.persBuffs[spec.name][j]));
                    }
                    res.push(data);
                }
                return res;
            }
        }
    });

    Vue.component("buff-stats-component", {
        props: ['datatypes', 'datatype', 'phaseindex', 'playerindex'],
        template: `${tmplBuffStats}`,
        data: function () {
            return {
                mode: 0,
                cache: new Map()
            };
        },
        computed: {
            singleGroup: function() {
               return logData.singleGroup;
            },
            phase: function() {
                return logData.phases[this.phaseindex];
            },
            boons: function () {
                var data = [];
                for (var i = 0; i < logData.boons.length; i++) {
                    data[i] = findSkill(true, logData.boons[i]);
                }
                return data;
            },
            offs: function () {
                var data = [];
                for (var i = 0; i < logData.offBuffs.length; i++) {
                    data[i] = findSkill(true, logData.offBuffs[i]);
                }
                return data;
            },
            defs: function () {
                var data = [];
                for (var i = 0; i < logData.defBuffs.length; i++) {
                    data[i] = findSkill(true, logData.defBuffs[i]);
                }
                return data;
            },
            buffData: function () {
                if (this.cache.has(this.phaseindex)) {
                    return this.cache.get(this.phaseindex);
                }
                var getData = function (stats, genself, gengroup, genoffgr, gensquad) {
                    var uptimes = [],
                        gens = [],
                        gengr = [],
                        genoff = [],
                        gensq = [];
                    var avg = [],
                        gravg = [],
                        totalavg = [];
                    var grcount = [],
                        totalcount = 0;
                    var grBoonAvg = [],
                        totalBoonAvg = 0;
                    var i, k;
                    for (i = 0; i < logData.players.length; i++) {
                        var player = logData.players[i];
                        if (player.isConjure) {
                            continue;
                        }
                        uptimes.push({
                            player: player,
                            data: stats[i]
                        });
                        gens.push({
                            player: player,
                            data: genself[i]
                        });
                        gengr.push({
                            player: player,
                            data: gengroup[i]
                        });
                        genoff.push({
                            player: player,
                            data: genoffgr[i]
                        });
                        gensq.push({
                            player: player,
                            data: gensquad[i]
                        });
                        if (!gravg[player.group]) {
                            gravg[player.group] = [];
                            grcount[player.group] = 0;
                            grBoonAvg[player.group] = 0;
                        }
                        totalcount++;
                        totalBoonAvg += stats[i].avg;
                        grBoonAvg[player.group] += stats[i].avg;
                        grcount[player.group]++;
                        for (var j = 0; j < stats[i].data.length; j++) {
                            totalavg[j] = (totalavg[j] || 0) + (stats[i].data[j][0] || 0);
                            gravg[player.group][j] = (gravg[player.group][j] || 0) + (stats[i].data[j][0] || 0);
                        }
                    }
                    for (i = 0; i < gravg.length; i++) {
                        if (gravg[i]) {
                            for (k = 0; k < gravg[i].length; k++) {
                                gravg[i][k] = Math.round(100 * gravg[i][k] / grcount[i]) / 100;
                            }
                            avg.push({
                                name: "Group " + i,
                                data: gravg[i],
                                avg: Math.round(10 * grBoonAvg[i] / grcount[i]) / 10
                            });
                        }
                    }
                    for (k = 0; k < totalavg.length; k++) {
                        totalavg[k] = Math.round(100 * totalavg[k] / totalcount) / 100;
                    }
                    avg.push({
                        name: "Total",
                        data: totalavg,
                        avg: Math.round(10 * totalBoonAvg / totalcount) / 10
                    });
                    return [uptimes, gens, gengr, genoff, gensq, avg];
                };
                var res = {
                    boonsData: getData(this.phase.boonStats, this.phase.boonGenSelfStats,
                        this.phase.boonGenGroupStats, this.phase.boonGenOGroupStats, this.phase.boonGenSquadStats),
                    offsData: getData(this.phase.offBuffStats, this.phase.offBuffGenSelfStats,
                        this.phase.offBuffGenGroupStats, this.phase.offBuffGenOGroupStats, this.phase.offBuffGenSquadStats),
                    defsData: getData(this.phase.defBuffStats, this.phase.defBuffGenSelfStats,
                        this.phase.defBuffGenGroupStats, this.phase.defBuffGenOGroupStats, this.phase.defBuffGenSquadStats)
                };
                this.cache.set(this.phaseindex, res);
                return res;
            }
        },
    });
};
