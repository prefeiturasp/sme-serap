﻿/**
 * function FormElectronicTestController Controller
 * @namespace Controller
 * @author Jessica Sartori 12/06/2017
 */
(function (angular, $) {

    'use strict';

    //~SETTER
    angular
        .module('appMain', ['services', 'filters', 'directives']);

    //~GETTER
    angular
        .module('appMain')
        .controller("FormElectronicTestController", FormElectronicTestController);


    FormElectronicTestController.$inject = ['$scope', '$notification', '$location', '$anchorScroll', '$util', '$timeout', 'ElectronicTestModel', '$window', '$sce'];


    function FormElectronicTestController(ng, $notification, $location, $anchorScroll, $util, $timeout, ElectronicTestModel, $window, $sce) {
        Init();

        function Init() {

            $notification.clear();
            ng.message = false;
            ng.test = null;
            ng.itens = null;
            ng.alternatives = null;
            ng.idsItens = null;
            ng.dadosAlternativaSelecionada = null;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL;
            load();
            ng.gabaritoAberto = false;
            ng.indiceItem = 0;
            ng.alternativaSelecionada = 0;
            ng.inicioProva = true;
            ng.pularRespondidas;
            ng.itensNaoPreenchidos = [];
            ng.itensPreenchidos = [];
            angular.element('#modalConfirmacaoEntregaProva').modal('hide');
            ng.mensagemEntregaProva;
            ng.admin = getCurrentVision() != EnumVisions.INDIVIDUAL ? true : false;
            ng.provaFinalizada = false;
            ng.showHeaderDetails = false;
            ng.enunciadoFontSize = 14;
            ng.alternativasFontSize = 14;
            ng.videos = null;
            ng.audios = null;
        };

        ng.loadHeaderDetais = function __loadHeaderDetais() {
            ng.showHeaderDetails = !ng.showHeaderDetails;
        };

        ng.abrirGabarito = function __abrirGabarito() {
            ng.gabaritoAberto = !ng.gabaritoAberto;
            if (ng.gabaritoAberto) {
                $('.gabarito-novo').addClass('abre');
            }
            else {
                $('.gabarito-novo').removeClass('abre');
            }
        };

        ng.proximaQuestao = function __proximaQuestao(item) {
            ng.itensPreenchidos = [];

            for (var i = 0; i < ng.alternatives.length ; i++) {
                if (ng.alternatives[i].Selected === true) {
                    ng.itensPreenchidos.push(ng.alternatives[i].ItemOrder);
                }
            }

            if (ng.itensPreenchidos.length !== 0 && ng.pularRespondidas) {
                var contador = 1;

                while (ng.itens.length > contador && ng.itensPreenchidos.contains(item.ItemOrder + contador)) {
                    contador++;
                }

                if ((ng.indiceItem + contador) >= ng.itens.length) {
                    ng.indiceItem = ng.itens.length - 1;
                }
                else {
                    ng.indiceItem = ng.indiceItem + contador;
                }
            }
            else {
                ng.indiceItem = ng.indiceItem + 1;
            }

            ng.possuiTextoBase = ng.itens[ng.indiceItem].BaseTextId > 0 && ng.itens[ng.indiceItem].BaseTextDescription != null && ng.itens[ng.indiceItem].BaseTextDescription != "";

        };

        ng.voltaQuestao = function __voltaQuestao(item) {
            ng.itensPreenchidos = [];

            for (var i = 0; i < ng.alternatives.length ; i++) {
                if (ng.alternatives[i].Selected === true) {
                    ng.itensPreenchidos.push(ng.alternatives[i].ItemOrder);
                }
            }

            if (ng.itensPreenchidos.length !== 0 && ng.pularRespondidas) {
                var contador = 1;
                var ordem = item.ItemOrder;

                while (ordem > 0 && ng.itensPreenchidos.contains(ordem - 1)) {
                    contador++;
                    ordem--;
                }

                if ((ng.indiceItem - contador) <= 0) {
                    ng.indiceItem = ng.indiceItem - ng.indiceItem;
                }
                else {
                    ng.indiceItem = ng.indiceItem - contador;
                }
            }
            else {
                ng.indiceItem = ng.indiceItem - 1;
            }

            ng.possuiTextoBase = ng.itens[ng.indiceItem].BaseTextId > 0 && ng.itens[ng.indiceItem].BaseTextDescription != null && ng.itens[ng.indiceItem].BaseTextDescription != "";
        };

        Array.prototype.contains = function (obj) {
            var i = this.length;
            while (i--) {
                if (this[i] == obj) {
                    return true;
                }
            }
            return false;
        }

        function load() {
            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            let testKeyStorage = GetTestStorageKey(ng.testId, ng.alu_id, ng.tur_id);
            let testFromStorage = localStorage.getItem(testKeyStorage);
            debugger;

            if (testFromStorage != null) {
                ng.test = testFromStorage;
                finalizeLoad();
            }
            else {
                ElectronicTestModel.loadByTestId({ test_id: ng.testId, alu_id: ng.aluId, tur_id: ng.turId }, function (result) {
                    if (result.success) {

                        if (result.test.Id > 0) {
                            ng.test = result.test;
                            localStorage.setItem(GetTestStorageKey(ng.testId, ng.alu_id, ng.tur_id), JSON.stringify(ng.test));
                            finalizeLoad();
                        }
                        else {
                            ng.message = true;
                            ng.test = null;
                        }
                    }
                    else {
                        $notification[result.type ? result.type : 'error'](result.message);
                    }
                });
            }
        };

        function finalizeLoad() {

            if (ng.test.LastAnswer != null && ng.itens.length > ng.test.LastAnswer + 1) {
                ng.indiceItem = result.ordemUltimaResposta + 1;
            }

            ng.possuiTextoBase = ng.test.Itens[ng.indiceItem].BaseTextId > 0 && ng.itens[ng.indiceItem].BaseTextDescription != null && ng.itens[ng.indiceItem].BaseTextDescription != "";
            ng.provaFinalizada = ng.test.Done;
            ng.inicioProva = false;
        }

        ng.handleRadioClick = function (chosenAlternative) {
            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            var answer = {
                ItemId: chosenAlternative.Item_Id,
                AlternativeId: chosenAlternative.Id,
                Changed: true
            };

            let keyStorage = GetAnswerStorageKey(ng.testId, ng.aluId, ng.turId, answer.ItemId);
            localStorage.setItem(keyStorage, JSON.stringify(answer));

            for (let i = 0; i < ng.alternatives.length; i++) {
                ng.alternatives[i].Selected = ng.alternatives.findIndex(x => ng.alternatives[i].Id == chosenAlternative.Id)
            }
        };

        function GetTestStorageKey(testId, aluId, turId) {
            return "test-" + testId + "-" + aluId + "-" + turId;
        }

        function GetAnswerStorageKey(testId, aluId, turId, itemId) {
            return "answerTest-" + testId + "-" + aluId + "-" + turId + "-" + itemId;
        }

        ng.zoomAlternativas = function (up) {
            if (up == true && ng.alternativasFontSize < 22)
                ng.alternativasFontSize = ng.alternativasFontSize + 4;
            
            if (up == false && ng.alternativasFontSize > 14)
                ng.alternativasFontSize = ng.alternativasFontSize - 4;

            $('label.alternativeElementClass p').css("font-size", ng.alternativasFontSize + "px");
        };

        ng.zoomEnunciado = function (up) {
            if (up == true && ng.enunciadoFontSize < 22)
                ng.enunciadoFontSize = ng.enunciadoFontSize + 4;

            if (up == false && ng.enunciadoFontSize > 14)
                ng.enunciadoFontSize = ng.enunciadoFontSize - 4;

            $('div.baseTextElementClass p').css("font-size", ng.enunciadoFontSize + "px");
            $('div.statementElementClass p').css("font-size", ng.enunciadoFontSize + "px");
        };

        ng.sair = function () {
            $window.location.href = '/ElectronicTest/Index';
        }

        ng.openModalEntregaProva = function () {

            ng.itensPreenchidos = [];

            for (var i = 0; i < ng.alternatives.length ; i++) {
                if (ng.alternatives[i].Selected === true) {
                    ng.itensPreenchidos.push(ng.alternatives[i].ItemOrder);
                }
            }

            ng.mensagemEntregaProva = '';
            if (ng.itensPreenchidos.length !== ng.itens.length) {

                ng.mensagemEntregaProva = "O(s) item(ns) ";

                for (var i = 0; i < ng.itens.length; i++) {
                    if (!ng.itensPreenchidos.contains(ng.itens[i].ItemOrder)) {
                        ng.mensagemEntregaProva += ng.itens[i].ItemOrder + 1 + ", ";
                    }
                }
                ng.mensagemEntregaProva = ng.mensagemEntregaProva.substring(0, (ng.mensagemEntregaProva.length - 2));

                ng.mensagemEntregaProva += " não está(ão) preenchido(s). Deseja entregar a prova mesmo assim?";
            }
            else {
                ng.mensagemEntregaProva = "Deseja entregar a prova?";
            }

            angular.element("#modalConfirmacaoEntregaProva").modal({ backdrop: 'static' });
        };

        ng.entregarProva = function __entregarProva() {

            ng.params = $util.getUrlParams();
            ng.testId = ng.params.TestId;
            ng.aluId = ng.params.AluId;
            ng.turId = ng.params.TurId;

            ElectronicTestModel.finalizeCorrection({ tur_id: ng.turId, test_id: ng.testId, alu_id: ng.aluId }, function (result) {
                if (result.success) {
                    $notification.success(result.message);
                    angular.element("#modalConfirmacaoEntregaProva").modal("hide");
                    $window.location.href = '/ElectronicTest/Index';
                }
                else {
                    $notification[result.type ? result.type : 'error'](result.message);
                }

            });


        };

        ng.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }
    };
})(angular, jQuery);