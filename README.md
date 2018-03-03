# HAVIO
Helicopter Agency for Variance International Operations

세계의 다양한 임무를 수행하는 민간 헬리콥터 에이전시 HAVIO입니다. <br/>
세계 최고의 실력을 갖춘 준비된 파일럿으로 HAVIO에 스카웃되어 여러 임무들을 수행합니다. 

# 스크립트 설명
HAVIO의 핵심인 헬리콥터 조작을 위한 스크립트는 다음과 같습니다. 

## HelicopterFlightController.cs
헬리콥터의 조작을 담당하는 스크립트입니다. 대부분의 기능은 이 스크립트에서 처리합니다. 
* 헬리콥터 컬렉티브 레버 조종(상/하 이동)
* 헬리콥터 사이클 스틱 조종(전/후/좌/우 이동)
* 헬리콥터 안티토크 페달 조종(좌/우 회전)
* 헬리콥터의 시동의 켜고 끄기

## HelicopterControlsBehaviour.cs
사용자가 입력한대로 가상 컨텐츠의 조종 장치가 움직이도록 하는 스크립트입니다. 
* 컬렉티브 레버의 전/후 회전
* 사이클 스틱의 전/후/좌/우 회전

## HelicopterRotorController.cs
헬리콥터의 회전 날개를 조종하는 스크립트입니다. 
* 메인 로터 블레이드의 회전
* 테일 로터 블레이드의 회전

## HelicopterSFXController.cs
헬리콥터에서 발생하는 효과음을 재생하는 스크립트입니다.  
* 시동을 켜고 끌 때 날개 및 엔진 효과음
* 컬렉티브 레버 조작시 날개 및 엔진 효과음
* 일정 속도 이상에서 기체의 흔들림 효과음