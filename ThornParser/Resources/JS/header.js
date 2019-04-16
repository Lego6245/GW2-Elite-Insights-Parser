/*jshint esversion: 6 */

var compileHeader = function () {
    Vue.component("encounter-component", {
        props: [],
        template: `${tmplEncounter}`,
        methods: {
            getResultText: function (success) {
                return success ? "Success" : "Failure";
            },
            getResultClass: function (success) {
                return success ? ["text-success"] : ["text-warning"];
            }
        },
        computed: {
            encounter: function () {
                var targets = [];
                for (var i = 0; i < logData.phases[0].targets.length; i++) {
                    var targetData = logData.targets[logData.phases[0].targets[i]];
                    targets.push(targetData);
                }

                var encounter = {
                    name: logData.fightName,
                    success: logData.success,
                    icon: logData.fightIcon,
                    duration: logData.encounterDuration,
                    targets: targets
                };
                return encounter;
            }
        }
    });

    Vue.component("phase-component", {
        props: ["phases"],
        template: `${tmplPhase}`,
        methods: {
            select: function (phase) {
                for (var i = 0; i < this.phases.length; i++) {
                    this.phases[i].active = false;
                }
                phase.active = true;
            },
            getPhaseData: function(id) {
                return logData.phases[id];
            }
        }
    });

    Vue.component("target-component", {
        props: ["targets", "phaseindex"],
        template: `${tmplTargets}`,
        computed: {
            phase: function() {
                return logData.phases[this.phaseindex];
            }
        },
        methods: {
            show: function (index) {
                return this.phase.targets.indexOf(index) !== -1;
            },
            getTargetData: function(id) {
                return logData.targets[id];
            }
        }
    });

    Vue.component("player-component", {
        props: ["playerindex", "players"],
        template: `${tmplPlayers}`,
        methods: {
            getIcon: function (path) {
                return urls[path];
            },
            select: function (id) {
                var oldStatus = this.players[id].active;
                for (var i = 0; i < this.players.length; i++) {
                    this.players[i].active = false;
                }
                this.players[id].active = !oldStatus;
            }
        },
        computed: {
            scoreExpl: function () {
                return "Scores are relative to the squad. 10 means that that player had the highest stat in the squad. 8 means that that player had between 80% and 89% of the highest scored player's stat."
            },
            groups: function () {
                var aux = [];
                var i = 0;
                for (i = 0; i < logData.players.length; i++) {
                    var playerData = logData.players[i];
                    if (playerData.isConjure) {
                        continue;
                    }
                    if (!aux[playerData.group]) {
                        aux[playerData.group] = [];
                    }
                    aux[playerData.group].push(playerData);
                }

                var noUndefinedGroups = [];
                for (i = 0; i < aux.length; i++) {
                    if (aux[i]) {
                        noUndefinedGroups.push(aux[i]);
                    }
                }
                return noUndefinedGroups;
            }
        }
    });
};