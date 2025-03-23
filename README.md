# Bus Data Convert
![C#](https://img.shields.io/badge/C%23-68217A?style=flat-square)
![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D7?style=flat-square)

대한민국 버스 데이터를 통합하여 노선ID를 기준삼아 Json 파일로 변환하는 프로그램입니다.  
서울, 경기도 버스 데이터의 통합이 가능합니다.

# 사용방법
1. 버스 데이터를 다운로드 합니다. 파일은 다음 링크에서 얻을 수 있습니다.

| 지역 | 링크 | 기타 |
| :--- | :--- | :--- |
| 서울 | https://data.seoul.go.kr/dataList/OA-1095/F/1/datasetView.do |  |
| 경기도 | https://www.data.go.kr/data/15080658/openapi.do#/API%20%EB%AA%A9%EB%A1%9D/getBaseInfoItemv2 | - 공공데이터포털 api 사용<br>- routeStationDownloadUrl 사용 |

2. 프로그램을 실행 후 데이터 선택 버튼을 눌러 버스 노선 데이터를 추가합니다.
3. 변환 시작 버튼을 눌러 저장 위치를 지정하면 변환 및 저장이 진행됩니다.

# 상세
### 입력 데이터 구조
- 서울: xlsx 확장자
- 경기도: 행 구분에 `^`, 열 구분에 `|`가 사용된 csv 형태의 txt 확장자

### 출력 데이터 구조
```
{
  "routeId": "노선ID",
  "routeName": "노선이름",
  "stationDataList": [
    {
      "stationId": "정류장ID",
      "stationName": "정류장이름",
      "posX": "X좌표",
      "posY": "Y좌표"
    },
    {
      "stationId": "정류장ID",
      "stationName": "정류장이름",
      "posX": "X좌표",
      "posY": "Y좌표"
    }
  ]
}
```
### 최근 확인일
외부 데이터를 사용하므로 작동이 원활하지 않을 수 있습니다.  
최근 확인일은 2024.03.24 입니다.