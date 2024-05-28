### cs-vault-bridge
- 초기 테스트 하던 프로젝트 입니다.

### cs-vault-bridge-web
- REST API application 으로 제작 하려했으나 DLL 파일 로드 오류로 인하여 실패한 프로젝트 입니다.

### cs-vault-bridge-console
- 최종적으로 선택된 console application으로 다음과 같은 명령어 형식을 가집니다.
- [<Target 이름>][<함수 이름>][<서버 IP>][<end-point 이름>][<함수 파라미터>]

## 모듈 간단 설명
# Parse
- 전달 받은 명령어 정보로 switch 분기문을 사용해 필요한 기능을 실행하는 모듈
- Reflection을 사용하여 객체의 정보를 불러와 함수를 실행하는 구조를 가지고 있습니다
- 간단 사용 예시

```c#
MethodInfo mi = connection.WebServiceManager.ItemService.GetType().GetTypeInfo().GetMethod("원하는 함수 명");
MethodInvokation( mi, connection.WebServiceMangager.ItemService );
```

- MethodInvokation은 Reflection을 사용하여 함수 호출을 하는 코드를 추출해둔 함수입니다.

# BaseService
- 생성시 볼트 서버 IP, 볼트 서버 이름, 볼트 계정 정보를 기본 생성자로 생성하는 클래스
- Vault SDK의 WebServiceManager에서 제공되는 여러 서비스들중 한번의 질의로 해결 되지 못하는 기능들을 구현하기 위한 클래스
- 기본적인 Login, Logout기능만 보유한 뼈대
#Updater
- Vault SDK 질의 문의 결과를 Spring Boot application으로 반환 해주는 모듈

```c#
Updater<반환 형> updater = new Updater<반환 형>( < boot server ip> );
updater.GenericPost( <end-point>, <반환 데이터> );
```


## Target 이름
1. item
2. folder
3. property
4. test
5. Vault<볼트 서비스 엔티티 이름>Service
6. Method<볼트 서비스 엔티티 이름>service

이중 5번은 볼트 SDK에서 제공하는 DB 질의 함수를 실행하는데 사용하며, 6번은 Vault SDK 질의 함수들의 반환타입, 함수 명, 인자 타입의 정보를 보고 싶을때 사용합니다.

## 명령어 예시

method ItemService http://localhost:8080 /post-items {}

method PropertyService http://localhost:8080 /post-methods {}

VaultItemService GetItemBOMAssociationsByItemIds http://localhost:8080 /post-bom {itemIds:[40454],recurse:false}

item GetItemMasters http://localhost:8080 /post-items {}

## Spring Boot 사용 예시

결과는 내가 열어둔 Controller end-point로 반환형이 JSONStringfy되어 전달 되므로 반환형을 받을수있도록 DTO를 생성하여 받으면 됩니다.

```java
@PostMapping(value="/post-items")
public getItems(@RequestBody VaultItemDTO [] items){

    /// statement ...

}

class VaultItemDTO{

// Item class fields in Vault SDK

}
```



