/*jshint esversion: 6 */
"use strict";
$.extend($.fn.dataTable.defaults, {
    searching: false,
    ordering: true,
    paging: false,
    retrieve: true,
    dom: "t"
});

// polyfill for string include
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/includes
if (!String.prototype.includes) {
    Object.defineProperty(String.prototype, "includes", {
        value: function (search, start) {
            if (typeof start !== 'number') {
                start = 0;
            }
            if (start + search.length > this.length) {
                return false;
            } else {
                return this.indexOf(search, start) !== -1;
            }
        }
    });
}

/*var lazyTableUpdater = null;
if ('IntersectionObserver' in window) {
    lazyTableUpdater = new IntersectionObserver(function (entries, observer) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                var id = entry.target.id;
                var table = $("#" + id);
                if ($.fn.dataTable.isDataTable(table)) {
                    table.DataTable().rows().invalidate('dom').draw();
                }
                observer.unobserve(entry.target);
            }
        });
    });
}*/
var themes = {
    "yeti": "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/yeti/bootstrap.min.css",
    "slate": "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/4.1.1/slate/bootstrap.min.css"
};

var urls = {
    Warrior: "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png",
    Berserker: "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png",
    Spellbreaker: "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png",
    Guardian: "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png",
    Dragonhunter: "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png",
    DragonHunter: "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png",
    Firebrand: "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png",
    Revenant: "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png",
    Herald: "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png",
    Renegade: "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png",
    Engineer: "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png",
    Scrapper: "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png",
    Holosmith: "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png",
    Ranger: "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png",
    Druid: "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png",
    Soulbeast: "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png",
    Thief: "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png",
    Daredevil: "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png",
    Deadeye: "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png",
    Elementalist: "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png",
    Tempest: "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png",
    Weaver: "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png",
    Mesmer: "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png",
    Chronomancer: "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png",
    Mirage: "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png",
    Necromancer: "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png",
    Reaper: "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png",
    Scourge: "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png",

    Unknown: "https://wiki.guildwars2.com/images/thumb/d/de/Sword_slot.png/40px-Sword_slot.png",
    Sword: "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png",
    Axe: "https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png",
    Dagger: "https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png",
    Mace: "https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png",
    Pistol: "https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png",
    Scepter: "https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png",
    Focus: "https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png",
    Shield: "https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png",
    Torch: "https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png",
    Warhorn: "https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png",
    Greatsword: "https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png",
    Hammer: "https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png",
    Longbow: "https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png",
    Shortbow: "https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png",
    Rifle: "https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png",
    Staff: "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png",
    Trident: "https://wiki.guildwars2.com/images/9/98/Crimson_Antique_Trident.png",
    Speargun: "https://wiki.guildwars2.com/images/3/3b/Crimson_Antique_Harpoon_Gun.png",
    Spear: "https://wiki.guildwars2.com/images/c/cb/Crimson_Antique_Impaler.png"
};

const specs = [
    "Warrior", "Berserker", "Spellbreaker", "Revenant", "Herald", "Renegade", "Guardian", "Dragonhunter", "Firebrand",
    "Ranger", "Druid", "Soulbeast", "Engineer", "Scrapper", "Holosmith", "Thief", "Daredevil", "Deadeye",
    "Mesmer", "Chronomancer", "Mirage", "Necromancer", "Reaper", "Scourge", "Elementalist", "Tempest", "Weaver"
];

const specToBase = {
    Warrior: 'Warrior',
    Berserker: 'Warrior',
    Spellbreaker: 'Warrior',
    Revenant: "Revenant",
    Herald: "Revenant",
    Renegade: "Revenant",
    Guardian: "Guardian",
    Dragonhunter: "Guardian",
    Firebrand: "Guardian",
    Ranger: "Ranger",
    Druid: "Ranger",
    Soulbeast: "Ranger",
    Engineer: "Engineer",
    Scrapper: "Engineer",
    Holosmith: "Engineer",
    Thief: "Thief",
    Daredevil: "Thief",
    Deadeye: "Thief",
    Mesmer: "Mesmer",
    Chronomancer: "Mesmer",
    Mirage: "Mesmer",
    Necromancer: "Necromancer",
    Reaper: "Necromancer",
    Scourge: "Necromancer",
    Elementalist: "Elementalist",
    Tempest: "Elementalist",
    Weaver: "Elementalist"
};

function findSkill(isBuff, id) {
    var skill;
    if (isBuff) {
        skill = buffMap['b' + id] || {};
    } else {
        skill = skillMap["s" + id] || {};
    }
    skill.condi = isBuff;
    return skill;
}

function getTargetCacheID(activetargets) {
    var id = 0;
    for (var i = 0; i < activetargets.length; i++) {
        id = id | Math.pow(2,activetargets[i]);
    }
    return id;
}

function computeRotationData(rotationData, images, data) {
    if (rotationData) {
        var rotaTrace = {
            x: [],
            base: [],
            y: [],
            name: 'Rotation',
            text: [],
            orientation: 'h',
            mode: 'markers',
            type: 'bar',
            width: [],
            hoverinfo: 'text',
            hoverlabel: {
                namelength: '-1'
            },
            marker: {
                color: [],
                width: '5',
                line: {
                    color: [],
                    width: '2'
                }
            },
            showlegend: false
        }
        for (var i = 0; i < rotationData.length; i++) {
            var item = rotationData[i];
            var x = item[0];
            var skillId = item[1];
            var duration = item[2];
            var endType = item[3];
            var quick = item[4];
            var skill = findSkill(false, skillId);
            var aa = false;
            var icon;
            var name = '???';
            if (skill) {
                aa = skill.aa;
                icon = skill.icon;
                name = skill.name;
            }

            if (!icon.includes("render")) {
                icon = null;
            }

            if (!aa && icon) {
                images.push({
                    source: icon,
                    xref: 'x',
                    yref: 'y',
                    x: x,
                    y: 0.0,
                    sizex: 1.1,
                    sizey: 1.1,
                    xanchor: 'middle',
                    yanchor: 'bottom'
                });
            }

            var fillColor;
            if (endType == 1) fillColor = 'rgb(40,40,220)';
            else if (endType == 2) fillColor = 'rgb(220,40,40)';
            else if (endType == 3) fillColor = 'rgb(40,220,40)';
            else fillColor = 'rgb(220,220,0)';

            rotaTrace.x.push(duration / 1000.0);
            rotaTrace.base.push(x);
            rotaTrace.y.push(1.2);
            rotaTrace.text.push(name + ' at ' + x + 's for ' + duration + 'ms');
            rotaTrace.width.push(aa ? 0.5 : 1.0);
            rotaTrace.marker.color.push(fillColor);
            rotaTrace.marker.line.color.push(quick ? 'rgb(220,40,220)' : 'rgb(20,20,20)');
        }
        data.push(rotaTrace);
        return 1;
    }
    return 0;
}

function computePhaseMarkupSettings(currentArea, areas, annotations) {
    var y = 1;
    var textbg = '#0000FF';
    var x = (currentArea.end + currentArea.start) / 2;
    for (var i = annotations.length - 1; i >= 0; i--) {
        var annotation = annotations[i];
        var area = areas[i];
        if ((area.start <= currentArea.start && area.end >= currentArea.end) || area.end >= currentArea.start - 2) {
            // current area included in area OR current area intersects area
            if (annotation.bgcolor === textbg) {
                textbg = '#FF0000';
            }
            y = annotation.y === y && area.end > currentArea.start ? 1.09 : y;
            break;
        }
    }
    return {
        y: y,
        x: x,
        textbg: textbg
    };
}

function computePhaseMarkups(shapes, annotations, phase, linecolor) {
    if (phase.markupAreas) {
        for (var i = 0; i < phase.markupAreas.length; i++) {
            var area = phase.markupAreas[i];
            var setting = computePhaseMarkupSettings(area, phase.markupAreas, annotations);
            annotations.push({
                x: setting.x,
                y: setting.y,
                xref: 'x',
                yref: 'paper',
                xanchor: 'center',
                yanchor: 'bottom',
                text: area.label + '<br>' + '(' + Math.round(1000 * (area.end - area.start)) / 1000 + ' s)',
                font: {
                    color: '#ffffff'
                },
                showarrow: false,
                bordercolor: '#A0A0A0',
                borderwidth: 2,
                bgcolor: setting.textbg,
                opacity: 0.8
            });
            if (area.highlight) {
                shapes.push({
                    type: 'rect',
                    xref: 'x',
                    yref: 'paper',
                    x0: area.start,
                    y0: 0,
                    x1: area.end,
                    y1: 1,
                    fillcolor: setting.textbg,
                    opacity: 0.2,
                    line: {
                        width: 0
                    },
                    layer: 'below'
                });
            }
        }
    }
    if (phase.markupLines) {
        for (var i = 0; i < phase.markupLines.length; i++) {
            var x = phase.markupLines[i];
            shapes.push({
                type: 'line',
                xref: 'x',
                yref: 'paper',
                x0: x,
                y0: 0,
                x1: x,
                y1: 1,
                line: {
                    color: linecolor,
                    width: 2,
                    dash: 'dash'
                },
                opacity: 0.6,
            });
        }
    }
}


function computePlayerDPS(player, damageData, lim, phasebreaks, activetargets, cacheID, lastTime) {
    if (player.dpsGraphCache.has(cacheID)) {
        return player.dpsGraphCache.get(cacheID);
    }
    var totalDamage = 0;
    var targetDamage = 0;
    var totalDPS = [0];
    var cleaveDPS = [0];
    var targetDPS = [0];
    var totalTotalDamage = [0];
    var totalCleaveDamage = [0];
    var totalTargetDamage = [0];
    var maxDPS = {
        total: 0,
        cleave: 0,
        target: 0
    };
    var end = damageData.total.length;
    if (lastTime > 0) {
        end--;
    }
    var j, limID = 0, targetid, k;
    for (j = 1; j < end; j++) {
        if (lim > 0) {
            limID = Math.max(j - lim, 0);
        } else if (phasebreaks && phasebreaks[j-1]) {
            limID = j;
        }
        var div = Math.max(j - limID, 1);
        totalDamage = damageData.total[j] - damageData.total[limID];
        targetDamage = 0;
        for (k = 0; k < activetargets.length; k++) {
            targetid = activetargets[k];
            targetDamage += damageData.targets[targetid][j] - damageData.targets[targetid][limID];
        }
        totalDPS[j] = Math.round(totalDamage / div);
        targetDPS[j] = Math.round(targetDamage / div);
        cleaveDPS[j] = Math.round((totalDamage - targetDamage) / div);
        totalTotalDamage[j] = totalDamage;
        totalTargetDamage[j] = targetDamage;
        totalCleaveDamage[j] = (totalDamage - targetDamage);
        maxDPS.total = Math.max(maxDPS.total, totalDPS[j]);
        maxDPS.target = Math.max(maxDPS.target, targetDPS[j]);
        maxDPS.cleave = Math.max(maxDPS.cleave, cleaveDPS[j]);
    }
    // last point management
    if (lastTime > 0) {
        if (lim > 0) {
            limID = Math.round(Math.max(lastTime - lim, 0));
        } else if (phasebreaks && phasebreaks[j-1]) {
            limID = j;
        }
        totalDamage = damageData.total[j] - damageData.total[limID];
        targetDamage = 0;
        for (k = 0; k < activetargets.length; k++) {
            targetid = activetargets[k];
            targetDamage += damageData.targets[targetid][j] - damageData.targets[targetid][limID];
        }
        totalDPS[j] = Math.round(totalDamage / (lastTime - limID));
        targetDPS[j] = Math.round(targetDamage / (lastTime - limID));
        cleaveDPS[j] = Math.round((totalDamage - targetDamage) / (lastTime - limID));
        totalTotalDamage[j] = totalDamage;
        totalTargetDamage[j] = targetDamage;
        totalCleaveDamage[j] = (totalDamage - targetDamage);
        maxDPS.total = Math.max(maxDPS.total, totalDPS[j]);
        maxDPS.target = Math.max(maxDPS.target, targetDPS[j]);
        maxDPS.cleave = Math.max(maxDPS.cleave, cleaveDPS[j]);
    }
    if (maxDPS.total < 1e-6) {
        maxDPS.total = 10;
    }
    if (maxDPS.target < 1e-6) {
        maxDPS.target = 10;
    }
    if (maxDPS.cleave < 1e-6) {
        maxDPS.cleave = 10;
    }
    var res = {
        dps: {
            total: totalDPS,
            target: targetDPS,
            cleave: cleaveDPS
        },
        total: {
            total: totalTotalDamage,
            target: totalTargetDamage,
            cleave: totalCleaveDamage
        },
        maxDPS: maxDPS
    };
    player.dpsGraphCache.set(cacheID, res);
    return res;
}

function getActorGraphLayout(images, color) {
    return {
        barmode: 'stack',
        yaxis: {
            title: 'Rotation',
            domain: [0, 0.09],
            fixedrange: true,
            showgrid: false,
            showticklabels: false,
            color: color,
            range: [0, 2]
        },
        legend: {
            traceorder: 'reversed'
        },
        hovermode: 'compare',
        hoverdistance: 1100,
        yaxis2: {
            title: 'Buffs',
            domain: [0.11, 0.6],
            color: color,
            gridcolor: color,
            fixedrange: true
        },
        yaxis3: {
            title: 'DPS',
            color: color,
            gridcolor: color,
            domain: [0.61, 1]
        },
        images: images,
        font: {
            color: color
        },
        xaxis: {
            title: 'Time(sec)',
            color: color,
            gridcolor: color,
            xrangeslider: {}
        },
        paper_bgcolor: 'rgba(0,0,0,0)',
        plot_bgcolor: 'rgba(0,0,0,0)',
        shapes: [],
        annotations: [],
        autosize: true,
        width: 1100,
        height: 800,
        datarevision: new Date().getTime(),
    };
}

function computeTargetHealthData(graph, targets, phase, data, yaxis, times) {
    for (var i = 0; i < graph.targets.length; i++) {
        var health = graph.targets[i].health;
        var hpTexts = [];
        var target = targets[phase.targets[i]];
        for (var j = 0; j < health.length; j++) {
            hpTexts[j] = health[j] + "% hp - " + target.name ;
        }
        var res = {
            x: times,
            text: hpTexts,
            mode: 'lines',
            line: {
                shape: 'spline',
                dash: 'dashdot'
            },
            hoverinfo: 'text+x',
            name: target.name + ' health',
        };
        if (yaxis) {
            res.yaxis = yaxis;
        }
        data.push(res);
    }
    return graph.targets.length;
}

function computeBuffData(buffData, data) {
    if (buffData) {
        for (var i = 0; i < buffData.length; i++) {
            var boonItem = buffData[i];
            var boon = findSkill(true, boonItem.id);
            var line = {
                x: [],
                y: [],
                text: [],
                yaxis: 'y2',
                type: 'scatter',
                visible: boonItem.visible ? null : 'legendonly',
                line: {
                    color: boonItem.color,
                    shape: 'linear'
                },
                hoverinfo: 'text+x',
                fill: 'tozeroy',
                name: boon.name.substring(0,20)
            };
            line.x.push(boonItem.states[0][0]);
            line.y.push(boonItem.states[0][1]);
            line.text.push(boon.name + ': ' + boonItem.states[0][1]);
            for (var p = 1; p < boonItem.states.length; p++) {
                line.x.push(boonItem.states[p][0]-0.001);
                line.y.push(boonItem.states[p-1][1]);
                line.text.push(boon.name + ': ' + boonItem.states[p-1][1]);
                line.x.push(boonItem.states[p][0]);
                line.y.push(boonItem.states[p][1]);
                line.text.push(boon.name + ': ' + boonItem.states[p][1]);
            }
            data.push(line);
        }
        return buffData.length;
    }
    return 0;
}

var initTable = function (id, cell, order, orderCallBack) {
    var table = $(id);
    if (!table.length) {
        return;
    }
    /*if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        var lazyTableObserver = new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    table.DataTable({
                        order: [
                            [cell, order]
                        ]
                    });
                    if (orderCallBack) {
                        table.DataTable().on('order.dt', orderCallBack);
                    }
                    observer.unobserve(entry.target);
                }
            });
        });
        lazyTableObserver.observe(lazyTable);
    } else {*/
    table.DataTable({
        order: [
            [cell, order]
        ]
    });
    if (orderCallBack) {
        table.DataTable().on('order.dt', orderCallBack);
    }
    //}
};

var updateTable = function (id) {
    /*if (lazyTableUpdater) {
        var lazyTable = document.querySelector(id);
        lazyTableUpdater.unobserve(lazyTable);
        lazyTableUpdater.observe(lazyTable);
    } else {*/
    var table = $(id);
    if ($.fn.dataTable.isDataTable(id)) {
        table.DataTable().rows().invalidate('dom');
        table.DataTable().draw();
    }
    //}
};

var DataTypes = {
    damageTable: 0,
    defTable: 1,
    supTable: 2,
    gameplayTable: 3,
    mechanicTable: 4,
    boonTable: 5,
    offensiveBuffTable: 6,
    defensiveBuffTable: 7,
    personalBuffTable: 8,
    playerTab: 9,
    targetTab: 10,
    dpsGraph: 11,
    dmgModifiersTable: 12,
};

/*function getActorGraphLayout(images, boonYs, stackingBoons) {
    var layout = {
        barmode: 'stack',
        yaxis: {
            title: 'Rotation',
            domain: [0, 0.1],
            fixedrange: true,
            showgrid: false,
            showticklabels: false,
            color: '#cccccc',
            range: [0, 2]
        },
        legend: {
            traceorder: 'reversed'
        },
        hovermode: 'compare',
        images: images,
        font: {
            color: '#cccccc'
        },
        xaxis: {
            title: 'Time(sec)',
            color: '#cccccc',
            gridcolor: '#cccccc',
            xrangeslider: {}
        },
        paper_bgcolor: 'rgba(0,0,0,0)',
        plot_bgcolor: 'rgba(0,0,0,0)',
        shapes: [],
        annotations: [],
        autosize: true,
        width: 1100,
        height: 1100,
        datarevision: new Date().getTime(),
    };
    layout['yaxis' + (2 + boonYs)] = {
        title: 'DPS',
        color: '#cccccc',
        gridcolor: '#cccccc',
        domain: [0.75, 1]
    };
    var perBoon = 0.65 / boonYs;
    var singleBuffs = boonYs;
    if (stackingBoons) {
        layout['yaxis' + (2 + boonYs - 1)] = {
            title: 'Stacking Buffs',
            color: '#cccccc',
            gridcolor: '#cccccc',
            domain: [0.70, 0.75]
        };
        perBoon = 0.6 / (boonYs - 1);
        singleBuffs--;
    }
    for (var i = 0; i < singleBuffs; i++) {
        layout['yaxis' + (2 + i)] = {
            title: '',
            color: '#cccccc',
            showgrid: false,
            showticklabels: false,
            domain: [0.1 + i * perBoon, 0.1 + (i + 1) * perBoon]
        };
    }
    return layout;
}*/

/*
function computeBuffData(buffData, data) {
    var ystart = 0;
    if (buffData) {
        var stackings = [];
        var i;
        for (i = buffData.length - 1; i >= 0; i--) {
            var boonItem = buffData[i];
            var boon = findSkill(true, boonItem.id);
            var line = {
                x: [],
                y: [],
                yaxis: boon.stacking ? 'stacking' : 'y' + (2 + ystart++),
                type: 'scatter',
                visible: boonItem.visible || !boon.stacking ? null : 'legendonly',
                line: {
                    color: boonItem.color,
                    shape: 'hv'
                },
                fill: boon.stacking ? 'tozeroy' : 'toself',
                name: boon.name,
                showlegend: boon.stacking ? true : false,
            };
            for (var p = 0; p < boonItem.states.length; p++) {
                line.x[p] = boonItem.states[p][0];
                line.y[p] = boonItem.states[p][1];
            }
            if (boon.stacking) {
                stackings.push(line);
            }
            data.push(line);
        }
        if (stackings.length) {
            var axis = 'y' + (2 + ystart++);
            for (i = 0; i < stackings.length; i++) {
                stackings[i].yaxis = axis;
            }
        }
        return {
            actorOffset: buffData.length,
            y: ystart,
            stacking: stackings.length > 0
        };
    }
    return {
        actorOffset: 0,
        y: 0,
        stacking: false
    };
}*/